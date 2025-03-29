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
    public interface ITradeService
    {
        Task<CryptoDto> BuyCryptoAsync(int userId, int cryptoId, int amount);
        Task<CryptoDto> SellCryptoAsync(int userId, int cryptoId, int amount);
        Task<PortfolioDto> GetPortfolio(int userId);
    }

    public class TradeService : ITradeService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        public TradeService(CryptoDbContext cryptoDbContext, IMapper mapper)
        {
            _context = cryptoDbContext;
            _mapper = mapper;
        }

        public async Task<CryptoDto> BuyCryptoAsync(int userId, int cryptoId, int amount)
        {
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == cryptoId);
            if(crypto == default)
            {
                throw new Exception("Crypto not found");
            }
            Wallet userWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if(userWallet == default)
            {
                throw new Exception("Wallet not found");
            }
            WalletCrypto walletcrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == userWallet.Id && wc.CryptoId==cryptoId);
            
            if(crypto.Available < amount)
            {
                throw new Exception("Not enough crypto available");
            }
            if (userWallet.Balance < crypto.Price * amount)
            {
                throw new Exception("Not enough balance");
            }
            userWallet.Balance -= crypto.Price * amount;
            crypto.Available -= amount;
            if (walletcrypto == default)
            {
                walletcrypto = new WalletCrypto { WalletId = userWallet.Id, CryptoId = crypto.Id, Amount = amount, BuyPrice = crypto.Price};
                await _context.WalletCrypto.AddAsync(walletcrypto);
            }
            else
            {
                walletcrypto.Amount += amount;
                walletcrypto.BuyPrice = crypto.Price;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var transaction = new TransactionLog
            {
                UserId = userId,
                User = user,
                CryptoId = cryptoId,
                Crypto = crypto,
                Amount = amount,
                PricePerUnit = crypto.Price,
                Type = TransactionType.Buy,
                Description = $"{user.Name} bought {amount} {crypto.Name} for {crypto.Price * amount} $",
                Timestamp = DateTime.Now
            };

            _context.TransactionLogs.Add(transaction);

            await _context.SaveChangesAsync();
            return _mapper.Map<CryptoDto>(crypto);
        }

        public async Task<CryptoDto> SellCryptoAsync(int userId, int cryptoId, int amount)
        {
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == cryptoId);
            if (crypto == default)
            {
                throw new Exception("Crypto not found");
            }
            Wallet userWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (userWallet == default)
            {
                throw new Exception("Wallet not found");
            }
            WalletCrypto walletcrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == userWallet.Id && wc.CryptoId == cryptoId);
            if(walletcrypto == default)
            {
                throw new Exception("Crypto not found in wallet");
            }
            if(walletcrypto.Amount < amount)
            {
                throw new Exception("Not enough crypto available");
            }
            crypto.Available += amount;
            userWallet.Balance += crypto.Price * amount;
            walletcrypto.Amount -= amount;
            walletcrypto.BuyPrice = crypto.Price;
            if (walletcrypto.Amount == 0)
            {
                _context.WalletCrypto.Remove(walletcrypto);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var transaction = new TransactionLog
            {
                UserId = userId,
                User = user,
                CryptoId = cryptoId,
                Crypto = crypto,
                Amount = amount,
                PricePerUnit = crypto.Price,
                Type = TransactionType.Sell,
                Description = $"{user.Name} sold {amount} {crypto.Name} for {crypto.Price * amount} $",
                Timestamp = DateTime.Now
            };

            _context.TransactionLogs.Add(transaction);

            await _context.SaveChangesAsync();
            return _mapper.Map<CryptoDto>(crypto);
        }
        public async Task<PortfolioDto> GetPortfolio(int userId)
        {
            var wallet = await _context.Wallets.Include(w => w.WalletCryptos).ThenInclude(c => c.Crypto).FirstOrDefaultAsync(w => w.UserId == userId);
            return _mapper.Map<PortfolioDto>(wallet);
        }
    }
}
