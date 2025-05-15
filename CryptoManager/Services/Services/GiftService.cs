using AutoMapper;
using BCrypt.Net;
using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IGiftService
    {
        Task GiftCryptoAsync(GiftDto dto);
        Task<List<GiftHistoryDto>> GetGiftHistoryAsync(int userId);
        //Task<IList<AlertDto>> GetActiveAlertsAsync(int userId);
        //Task<IList<AlertLogDto>> GetAlertsHistoryAsync(int userId);
        //Task DeleteAlertAsync(int alertId);
    }
    public class GiftService : IGiftService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public GiftService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task GiftCryptoAsync(GiftDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validálás
                if (dto.FromUserId == dto.ToUserId)
                    throw new InvalidOperationException("Nem ajándékozhatsz magadnak.");

                if (dto.Amount <= 0)
                    throw new InvalidOperationException("Az összegnek pozitívnak kell lennie.");

                var senderUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.FromUserId);
                var recieverUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.ToUserId);
                if (senderUser == null || recieverUser == null)
                {
                    throw new Exception("User not found");
                }
                // Küldő és fogadó felhasználó egyenleg lekérése
                var senderWallett = await _context.Wallets.FirstOrDefaultAsync(w =>
                    w.UserId == dto.FromUserId);
                var senderBalance = await _context.WalletCrypto.FirstOrDefaultAsync(w =>
                    w.WalletId == senderWallett.Id && w.CryptoId == dto.CryptoId);

                var recieverWallett = await _context.Wallets.FirstOrDefaultAsync(w =>
                    w.UserId == dto.ToUserId);
                var recieverBalance = await _context.WalletCrypto.FirstOrDefaultAsync(w =>
                    w.WalletId == recieverWallett.Id && w.CryptoId == dto.CryptoId);

                if (senderBalance == null || senderBalance.Amount < dto.Amount)
                    throw new InvalidOperationException("Nincs elég kriptovalutád az ajándékozáshoz.");

                // Levonás
                senderBalance.Amount -= dto.Amount;

                // Jóváírás
                if (recieverBalance == default)
                {
                    recieverBalance = new WalletCrypto { WalletId = recieverWallett.Id, CryptoId = dto.CryptoId, Amount = dto.Amount, BuyPrice = 0 };
                    await _context.WalletCrypto.AddAsync(recieverBalance);
                }
                else
                {
                    recieverBalance.Amount += dto.Amount;
                }
                var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == dto.CryptoId);
                // Tranzakció naplózása (mindkét fél részére, ha szükséges)
                var timestamp = DateTime.UtcNow;
                _context.TransactionLogs.Add(new TransactionLog
                {
                    UserId = dto.FromUserId,
                    CryptoId = dto.CryptoId,
                    Amount = dto.Amount,
                    BasePrice = 0,
                    TotalPrice = 0,
                    FeePrice = 0,
                    Type = TransactionType.Gift,
                    CurrentCryptoPrice = crypto.Price,
                    Description = $"Ajándékozás {dto.ToUserId} részére",
                    Timestamp = timestamp
                });

                _context.TransactionLogs.Add(new TransactionLog
                {
                    UserId = dto.ToUserId,
                    CryptoId = dto.CryptoId,
                    Amount = dto.Amount,
                    BasePrice = 0,
                    TotalPrice = 0,
                    FeePrice = 0,
                    Type = TransactionType.GiftReceived,
                    CurrentCryptoPrice = crypto.Price,
                    Description = $"Ajándék kapva {dto.FromUserId}-től",
                    Timestamp = timestamp
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<List<GiftHistoryDto>> GetGiftHistoryAsync(int userId)
        {
            var giftLogs = await _context.TransactionLogs
                .Include(t => t.Crypto)
                .Include(t => t.User)
                .Where(t =>
                    (t.UserId == userId && (t.Type == TransactionType.Gift || t.Type == TransactionType.GiftReceived)))
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();

            var result = new List<GiftHistoryDto>();

            foreach (var log in giftLogs)
            {
                // Ellenkező fél keresése a napló alapján (Description-ben benne van az ID)
                var counterpartyId = 0;
                string direction = "";
                if (log.Type == TransactionType.Gift)
                {
                    direction = "Sent";
                    var toUserId = ExtractUserIdFromDescription(log.Description);
                    counterpartyId = toUserId;
                }
                else if (log.Type == TransactionType.GiftReceived)
                {
                    direction = "Received";
                    var fromUserId = ExtractUserIdFromDescription(log.Description);
                    counterpartyId = fromUserId;
                }

                result.Add(new GiftHistoryDto
                {
                    Direction = direction,
                    CounterPartId = counterpartyId,
                    CryptoName = log.Crypto.Name,
                    Amount = log.Amount,
                    PriceAtGiftTime = log.PricePerUnit,
                    CurrentPrice = log.CurrentCryptoPrice != null ? (decimal)log.CurrentCryptoPrice : 0,
                    Timestamp = log.Timestamp
                });
            }

            return result;
        }

        // Helper: ID kinyerése a Description mezőből
        private int ExtractUserIdFromDescription(string? description)
        {
            if (string.IsNullOrEmpty(description))
                return 0;

            var parts = description.Split(' ');
            foreach (var part in parts)
            {
                if (int.TryParse(part, out var id))
                    return id;
            }

            return 0;
        }
    }
}

