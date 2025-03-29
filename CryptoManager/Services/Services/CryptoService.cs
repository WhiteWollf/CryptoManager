using AutoMapper;
using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface ICryptoService
    {
        Task<IList<CryptoDto>> GetAllCryptosAsync();
        Task<CryptoDto> GetCryptoByIdAsync(int cryptoId);
        Task<CryptoDto> AddCryptoAsync(CryptoDto newCrypto);
        Task DeleteCryptoAsync(int cryptoId);
    }
    public class CryptoService : ICryptoService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;
        public CryptoService(CryptoDbContext cryptoDbContext, IMapper mapper) { 
            _context = cryptoDbContext;
            _mapper = mapper;
        }

        public async Task<IList<CryptoDto>> GetAllCryptosAsync()
        {
            return await _context.Cryptos.Select(c => _mapper.Map<CryptoDto>(c)).ToListAsync();
        }

        public async  Task<CryptoDto> GetCryptoByIdAsync(int cryptoId)
        {
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == cryptoId);
            if(crypto == null)
            {
                throw new Exception("Crypto not found");
            }
            return _mapper.Map<CryptoDto>(crypto);
        }
        public async Task<CryptoDto> AddCryptoAsync(CryptoDto newCrypto)
        {
            if (_context.Cryptos.Count() >= 15)
            {
                throw new Exception("Maximum number of cryptos reached");
            }
            var crypto = _mapper.Map<Crypto>(newCrypto);
            await _context.Cryptos.AddAsync(crypto);
            await _context.SaveChangesAsync();
            return _mapper.Map<CryptoDto>(crypto);
        }

        public async Task DeleteCryptoAsync(int cryptoId)
        {
            var crypto = _context.Cryptos.FirstOrDefault(c => c.Id == cryptoId);
            if (crypto == null)
            {
                throw new Exception("Crypto not found");
            }
            _context.Cryptos.Remove(crypto);
            await _context.SaveChangesAsync();
        }
    }
}
