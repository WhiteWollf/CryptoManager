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
        Task<TransactionDto> BuyCryptoAsync(int userId, CryptoBuySellDto cryptoBuyDto);
        Task<TransactionDto> SellCryptoAsync(int userId, CryptoBuySellDto cryptoBuyDto);
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

        public async Task<TransactionDto> BuyCryptoAsync(int userId, CryptoBuySellDto cryptoBuyDto)
        {
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == cryptoBuyDto.Symbol);
            if(crypto == default)
            {
                throw new Exception("Crypto not found");
            }
            Wallet userWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if(userWallet == default)
            {
                throw new Exception("Wallet not found");
            }

            WalletCrypto walletcrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == userWallet.Id && wc.CryptoId==crypto.Id);
            if(crypto.Available < cryptoBuyDto.Amount)
            {
                throw new Exception("Not enough crypto available");
            }
            if (userWallet.Balance < crypto.Price * cryptoBuyDto.Amount)
            {
                throw new Exception("Not enough balance");
            }
            userWallet.Balance -= crypto.Price * cryptoBuyDto.Amount;
            crypto.Available -= cryptoBuyDto.Amount;
            if (walletcrypto == default)
            {
                walletcrypto = new WalletCrypto { WalletId = userWallet.Id, CryptoId = crypto.Id, Amount = cryptoBuyDto.Amount, BuyPrice = crypto.Price};
                await _context.WalletCrypto.AddAsync(walletcrypto);
            }
            else
            {
                walletcrypto.Amount += cryptoBuyDto.Amount;
                walletcrypto.BuyPrice = crypto.Price;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var transaction = new TransactionLog
            {
                UserId = userId,
                User = user,
                CryptoId = crypto.Id,
                Crypto = crypto,
                Amount = cryptoBuyDto.Amount,
                PricePerUnit = crypto.Price,
                Type = TransactionType.Buy,
                Description = $"{user.Name} bought {cryptoBuyDto.Amount} {crypto.Name} for {crypto.Price * cryptoBuyDto.Amount} $",
                Timestamp = DateTime.Now
            };

            _context.TransactionLogs.Add(transaction);

            await _context.SaveChangesAsync();
            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<TransactionDto> SellCryptoAsync(int userId, CryptoBuySellDto cryptoBuySellDto)
        {
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == cryptoBuySellDto.Symbol);
            if (cryptoBuySellDto.Amount < 0)
            {
                throw new Exception("Insufficent amount");
            }
            if (crypto == default)
            {
                throw new Exception("Crypto not found");
            }
            Wallet userWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (userWallet == default)
            {
                throw new Exception("Wallet not found");
            }
            WalletCrypto walletcrypto = await _context.WalletCrypto.FirstOrDefaultAsync(wc => wc.WalletId == userWallet.Id && wc.CryptoId == crypto.Id);
            if(walletcrypto == default)
            {
                throw new Exception("Crypto not found in wallet");
            }
            /*if(walletcrypto.Amount < cryptoBuySellDto.Amount)
            {
                throw new Exception("Not enough crypto available");
            }*/

            //Új ellenőrzés
            if(walletcrypto.Amount-walletcrypto.LockedAmount < cryptoBuySellDto.Amount)
            {
                throw new Exception("Not enough crypto available");
            }

            crypto.Available += cryptoBuySellDto.Amount;
            userWallet.Balance += crypto.Price * cryptoBuySellDto.Amount;
            walletcrypto.Amount -= cryptoBuySellDto.Amount;
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
                CryptoId = crypto.Id,
                Crypto = crypto,
                Amount = cryptoBuySellDto.Amount,
                PricePerUnit = crypto.Price,
                Type = TransactionType.Sell,
                Description = $"{user.Name} sold {cryptoBuySellDto.Amount} {crypto.Name} for {crypto.Price * cryptoBuySellDto.Amount} $",
                Timestamp = DateTime.Now
            };

            _context.TransactionLogs.Add(transaction);

            await _context.SaveChangesAsync();
            return _mapper.Map<TransactionDto>(transaction);
        }
        public async Task<PortfolioDto> GetPortfolio(int userId)
        {
            var wallet = await _context.Wallets.Include(w => w.WalletCryptos).ThenInclude(c => c.Crypto).FirstOrDefaultAsync(w => w.UserId == userId);
            return _mapper.Map<PortfolioDto>(wallet);
        }
    }
}
