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
    public interface IWalletService
    {
        Task<Wallet> GetWalletAsync(int userId);
        Task<WalletDto> UpdateWalletBalanceAsync(int userId, decimal newBalance);
        Task DeleteWalletAsync(int userId);
    }
    public class WalletService : IWalletService
    {
        private  readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public WalletService(CryptoDbContext cryptoDbContext, IMapper mapper)
        {
            _context = cryptoDbContext;
            _mapper = mapper;
        }
        public async Task<Wallet> GetWalletAsync(int userId)
        {
            var wallet = await _context.Wallets.Include(w=>w.Cryptos).ThenInclude(c=>c.Crypto).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null) 
                return null;
            return wallet;
        }

        public async Task<WalletDto> UpdateWalletBalanceAsync(int userId, decimal newBalance)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet != null) wallet.Balance = newBalance;
            await _context.SaveChangesAsync();
            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task DeleteWalletAsync(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if(wallet == default)
            {
                throw new Exception("Wallet not found");
            }
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
