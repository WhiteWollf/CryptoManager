using AutoMapper;
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
    public interface ISavingLockService
    {
        Task<string> CreateSavingLockAsync(SavingLockCreateDto dto);
        Task UpdateCryptoInterestRateAsync(CryptoInterestRateUpdateDto dto);
        Task<IList<CryptoInterestRateDto>> GetAllInterestRatesAsync();
        Task<UserSavingLockDto> GetUserSavingLocksAsync(int userId);
        Task<string> UnlockSavingLockAsync(int lockId);
    }
    public class SavingLockService : ISavingLockService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public SavingLockService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> CreateSavingLockAsync(SavingLockCreateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == default)
            {
                throw new Exception("User not found");
            }
            if (dto.EndDate < DateTime.UtcNow)
            {
                throw new Exception("Incorrect Date");
            }
            if (dto.Amount < 0)
            {
                throw new Exception("Incorrect Amount");
            }
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == dto.CryptoId);
            if (crypto == default)
            {
                throw new Exception("Incorrect Crypto");
            }

            var userWallett = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            var userWallettCrypto = await _context.WalletCrypto.FirstOrDefaultAsync(w => w.WalletId == userWallett.Id && w.CryptoId == crypto.Id);
            if (userWallettCrypto == default || (userWallettCrypto.Amount - userWallettCrypto.LockedAmount) < dto.Amount)
            {
                throw new Exception("Not enough Crypto available in wallett");
            }

            var currentInterest = await _context.CryptoInterestRates.FirstOrDefaultAsync(c => c.CryptoId == dto.CryptoId);
            if(currentInterest == default)
            {
                throw new Exception("No interest rate available on the given Crypto");
            }
            var newSavingLock = new SavingLock() { CryptoId = dto.CryptoId, UserId = dto.UserId, Amount = dto.Amount, StartDate = DateTime.UtcNow, EndDate = dto.EndDate, TotalAmount = dto.Amount, InterestRate = currentInterest.InterestRate };

            userWallettCrypto.LockedAmount += dto.Amount    ;

            await _context.SavingLocks.AddAsync(newSavingLock);
            await _context.SaveChangesAsync();

            var lockDays = Math.Round((dto.EndDate - DateTime.UtcNow).TotalDays);
            var expectedTotalAmount = dto.Amount * (decimal)Math.Pow((double)(1 + (currentInterest.InterestRate/100)), lockDays);

            return $"Várható összeg a kamatidő végén ({lockDays} nap): {Math.Round(expectedTotalAmount, 6)}";
        }

        public async Task<UserSavingLockDto> GetUserSavingLocksAsync(int userId)
        {
            var now = DateTime.UtcNow;

            var locks = await _context.SavingLocks
                .Include(s => s.Crypto)
                .Where(s => s.UserId == userId)
                .ToListAsync();

            var activeLocks = locks
                .Where(s => s.EndDate > now && s.IsActive)
                .Select(s => new SavingLockDto
                {
                    Id = s.Id,
                    CryptoName = s.Crypto.Name,
                    Amount = s.Amount,
                    TotalAmount = s.TotalAmount,
                    InterestRate = s.InterestRate,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive
                })
                .ToList();

            var expiredLocks = locks
                .Where(s => s.EndDate <= now || !s.IsActive)
                .Select(s => new SavingLockDto
                {
                    Id = s.Id,
                    CryptoName = s.Crypto.Name,
                    Amount = s.Amount,
                    TotalAmount = s.TotalAmount,
                    InterestRate = s.InterestRate,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive
                })
                .ToList();

            return new UserSavingLockDto
            {
                ActiveLocks = activeLocks,
                ExpiredLocks = expiredLocks
            };
        }

        public async Task UpdateCryptoInterestRateAsync(CryptoInterestRateUpdateDto dto)
        {
            if (dto.NewInterestRate < 0)
                throw new Exception("Interest rate must be positive.");

            var rate = await _context.CryptoInterestRates
                .FirstOrDefaultAsync(r => r.CryptoId == dto.CryptoId);

            if (rate == null)
                throw new Exception("Interest rate entry not found for the specified crypto.");

            rate.InterestRate = dto.NewInterestRate;
            await _context.SaveChangesAsync();
        }

        public async Task<IList<CryptoInterestRateDto>> GetAllInterestRatesAsync()
        {
            var rates = await _context.CryptoInterestRates
                .Include(r => r.Crypto)
                .ToListAsync();

            return rates.Select(r => new CryptoInterestRateDto
            {
                CryptoId = r.CryptoId,
                CryptoName = r.Crypto.Name,
                InterestRate = r.InterestRate
            }).ToList();
        }

        public async Task<string> UnlockSavingLockAsync(int lockId)
        {
            var saving = await _context.SavingLocks
                .Include(s => s.Crypto)
                .FirstOrDefaultAsync(s => s.Id == lockId);

            if (saving == null)
                throw new Exception("Saving not found");

            if (!saving.IsActive)
                throw new Exception("This saving is already inactive.");

            decimal interestProfit = saving.TotalAmount - saving.Amount;
            decimal tax = interestProfit * (decimal)0.15;

            var wallet = await _context.Wallets
                .Include(w => w.WalletCryptos)
                .FirstOrDefaultAsync(w => w.UserId == saving.UserId);

            var walletCrypto = wallet.WalletCryptos
                .FirstOrDefault(wc => wc.CryptoId == saving.CryptoId);

            if (walletCrypto == null) //nem fordulhat elő elméletileg
            {
                walletCrypto = new WalletCrypto
                {
                    WalletId = wallet.Id,
                    CryptoId = saving.CryptoId,
                    Amount = 0,
                    BuyPrice = 0,
                    LockedAmount = saving.Amount
                };
                _context.WalletCrypto.Add(walletCrypto);
            }

            walletCrypto.Amount += saving.Amount + interestProfit - tax;

            saving.IsActive = false;

            await _context.SaveChangesAsync();

            return $"Unlocked early. Earned interest: {interestProfit}, tax: {tax}, net interest added: {interestProfit-tax}";
        }
    }
}
