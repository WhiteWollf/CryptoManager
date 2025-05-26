using AutoMapper;
using BCrypt.Net;
using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IGiftService
    {
        Task<string> GiftListingAsync(GiftDto dto);
        Task<string> AcceptGiftAsync(int giftId, bool accepted);
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

        public async Task<string> GiftListingAsync(GiftDto dto)
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
                // Küldő felhasználó egyenleg lekérése
                var senderWallett = await _context.Wallets.FirstOrDefaultAsync(w =>
                    w.UserId == dto.FromUserId);
                var senderBalance = await _context.WalletCrypto.FirstOrDefaultAsync(w =>
                    w.WalletId == senderWallett.Id && w.CryptoId == dto.CryptoId);

                if (senderBalance == null || senderBalance.Amount < dto.Amount)
                    throw new InvalidOperationException("Nincs elég kriptovalutád az ajándékozáshoz.");

                senderBalance.LockedAmount += dto.Amount;
                var newGiftListing = new GiftListing() { SenderUserId = dto.FromUserId, RecieverUserId = dto.ToUserId, Amount = dto.Amount, CryptoId = dto.CryptoId };
                await _context.GiftListings.AddAsync(newGiftListing);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"Ajándékozási kérelem sikeresen létrehozva. {newGiftListing.Id}";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> AcceptGiftAsync(int giftId, bool accepted)
        {
            var giftListing = await _context.GiftListings.FirstOrDefaultAsync(g => g.Id == giftId);
            if (giftListing == default)
            {
                throw new Exception("A megadott ajándék nem található");
            }
            //Ha elfogadja az ajándékot
            if (accepted)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var senderUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == giftListing.SenderUserId);
                    var recieverUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == giftListing.RecieverUserId);
                    if(senderUser == default || recieverUser == default)
                    {
                        throw new Exception("Hiba, felhasználó nem találva!");
                    }
                    // Küldő és fogadó felhasználó egyenleg lekérése
                    var senderWallett = await _context.Wallets.FirstOrDefaultAsync(w =>
                        w.UserId == giftListing.SenderUserId);
                    var senderBalance = await _context.WalletCrypto.FirstOrDefaultAsync(w =>
                        w.WalletId == senderWallett.Id && w.CryptoId == giftListing.CryptoId);

                    var recieverWallett = await _context.Wallets.FirstOrDefaultAsync(w =>
                        w.UserId == giftListing.RecieverUserId);
                    var recieverBalance = await _context.WalletCrypto.FirstOrDefaultAsync(w =>
                        w.WalletId == recieverWallett.Id && w.CryptoId == giftListing.CryptoId);

                    if (senderBalance == null || senderBalance.Amount < giftListing.Amount)
                        throw new InvalidOperationException("Nincs elég kriptovalutád az ajándékozáshoz.");

                    // Levonás
                    senderBalance.LockedAmount -= giftListing.Amount;
                    senderBalance.Amount -= giftListing.Amount;

                    // Jóváírás
                    if (recieverBalance == default)
                    {
                        recieverBalance = new WalletCrypto { WalletId = recieverWallett.Id, CryptoId = giftListing.CryptoId, 
                            Amount = giftListing.Amount, BuyPrice = 0 };
                        await _context.WalletCrypto.AddAsync(recieverBalance);
                    }
                    else
                    {
                        recieverBalance.Amount += giftListing.Amount;
                    }
                    var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == giftListing.CryptoId);

                    if (senderBalance.Amount == 0)
                    {
                        _context.WalletCrypto.Remove(senderBalance);
                    }
                    // Tranzakció naplózása (mindkét fél részére)
                    var timestamp = DateTime.UtcNow;
                    _context.TransactionLogs.Add(new TransactionLog
                    {
                        UserId = giftListing.SenderUserId,
                        CryptoId = giftListing.CryptoId,
                        Amount = giftListing.Amount,
                        BasePrice = crypto.Price,
                        TotalPrice = crypto.Price * giftListing.Amount,
                        FeePrice = 0,
                        Type = TransactionType.Gifted,
                        CurrentCryptoPrice = crypto.Price,
                        Description = $"Ajándékozás {recieverUser.Name} ({giftListing.RecieverUserId}) részére",
                        Timestamp = timestamp
                    });

                    _context.TransactionLogs.Add(new TransactionLog
                    {
                        UserId = giftListing.RecieverUserId,
                        CryptoId = giftListing.CryptoId,
                        Amount = giftListing.Amount,
                        BasePrice = crypto.Price,
                        TotalPrice = crypto.Price * giftListing.Amount,
                        FeePrice = 0,
                        Type = TransactionType.GiftReceived,
                        CurrentCryptoPrice = crypto.Price,
                        Description = $"Ajándék kapva {senderUser.Name} ({giftListing.SenderUserId})-től",
                        Timestamp = timestamp
                    });

                    _context.GiftListings.Remove(giftListing);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return "Ajándék sikeresen elfogadva!";
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else  //Ha elutasítja az ajándékot
            {
                var senderWallett = await _context.Wallets.FirstOrDefaultAsync(w =>
                        w.UserId == giftListing.SenderUserId);
                var senderBalance = await _context.WalletCrypto.FirstOrDefaultAsync(w =>
                    w.WalletId == senderWallett.Id && w.CryptoId == giftListing.CryptoId);

                senderBalance.LockedAmount -= giftListing.Amount;
                _context.GiftListings.Remove(giftListing);
                await _context.SaveChangesAsync();

                return "Ajándék elutasítva!";
            }

        }
        public async Task<List<GiftHistoryDto>> GetGiftHistoryAsync(int userId)
        {
            var giftLogs = await _context.TransactionLogs
                .Include(t => t.Crypto)
                .Include(t => t.User)
                .Where(t =>
                    (t.UserId == userId && (t.Type == TransactionType.Gifted || t.Type == TransactionType.GiftReceived)))
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();

            var result = new List<GiftHistoryDto>();

            foreach (var log in giftLogs)
            {
                // Ellenkező fél keresése a napló alapján (Description-ben benne van az ID)
                var counterpartyId = 0;
                string direction = "";
                if (log.Type == TransactionType.Gifted)
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

