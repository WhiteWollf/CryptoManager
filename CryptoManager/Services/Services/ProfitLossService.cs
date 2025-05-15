using AutoMapper;
using DataContext.Context;
using DataContext.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IProfitLossService
    {
        Task<TotalProfitLossDto> TotalProfitLossAsync(int userId);
        Task<IList<CryptoProfitLossDto>> DetailedProfitLossAsync(int userId);
    }
    public class ProfitLossService : IProfitLossService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public ProfitLossService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IList<CryptoProfitLossDto>> DetailedProfitLossAsync(int userId)
        {
            var wallet = await _context.Wallets.Include(w => w.WalletCryptos).ThenInclude(c => c.Crypto).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }
            if (wallet.WalletCryptos.Count() == 0)
            {
                throw new Exception("Wallet is empty");
            }
            List<CryptoProfitLossDto> cryptoProfitLossDtos = new List<CryptoProfitLossDto>();
            foreach (var item in wallet.WalletCryptos)
            {
                decimal buyValue = item.BuyPrice == 0 ? 0 : item.BuyPrice * item.Amount;
                decimal currentValue = item.Crypto.Price * item.Amount;

                cryptoProfitLossDtos.Add(new CryptoProfitLossDto()
                {
                    CryptoName = item.Crypto.Name,
                    Symbol = item.Crypto.Symbol,
                    BuyValue = buyValue,
                    CurrentValue = currentValue,
                    ProfitLoss = Math.Round(item.Crypto.Price * item.Amount - buyValue, 2),
                    ProfitLossPercentage = item.BuyPrice == 0 ? 1 : Math.Round((item.Crypto.Price / item.BuyPrice) - 1, 2)
                });
            }
            return cryptoProfitLossDtos;
        }

        public async Task<TotalProfitLossDto> TotalProfitLossAsync(int userId)
        {
            var wallet = await _context.Wallets.Include(w => w.WalletCryptos).ThenInclude(c => c.Crypto).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }
            if(wallet.WalletCryptos.Count() == 0)
            {
                throw new Exception("Wallet is empty");
            }
            decimal profitloss = wallet.WalletCryptos.Sum(wc => wc.Crypto.Price * wc.Amount - (wc.BuyPrice == 0 ? 0 : wc.BuyPrice * wc.Amount));
            decimal buyValue = wallet.WalletCryptos.Sum(wc => wc.BuyPrice == 0 ? 0 : wc.BuyPrice * wc.Amount);
            decimal currentValue = wallet.WalletCryptos.Sum(wc => wc.Crypto.Price * wc.Amount);

            return new TotalProfitLossDto()
            {
                TotalProfitLoss = Math.Round(profitloss, 2),
                TotalProfitLossPercentage = Math.Round((currentValue / buyValue) - 1, 2)
            };

        }
    }
}
