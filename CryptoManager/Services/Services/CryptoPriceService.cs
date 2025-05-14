using AutoMapper;
using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface ICryptoPriceService
    {
        Task<CryptoChangeDto> UpdateCryptoPriceAsync(CryptoPriceChangeDto cryptoPriceChangeDto);
        Task<IList<CryptoPriceLogDto>> GetCryptoChangesAsync(int cryptoId);
    }
    public class CryptoPriceService : ICryptoPriceService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public CryptoPriceService(CryptoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [Authorize("Admin")]
        public async Task<CryptoChangeDto> UpdateCryptoPriceAsync(CryptoPriceChangeDto cryptoPriceChangeDto)
        {
            var crypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Id == cryptoPriceChangeDto.CryptoId);
            if (crypto == null)
            {
                throw new Exception("Crypto not found");
            }
            var cryptoChangeDto = new CryptoChangeDto()
            {
                Name = crypto.Name,
                Symbol = crypto.Symbol,
                OldPrice = crypto.Price,
                NewPrice = cryptoPriceChangeDto.NewPrice,
                Available = crypto.Available
            };
            crypto.Price = cryptoPriceChangeDto.NewPrice;

            _context.CryptoPriceLogs.Add(new CryptoPriceLog
            {
                CryptoId = crypto.Id,
                OldPrice = cryptoChangeDto.OldPrice,
                NewPrice = cryptoChangeDto.NewPrice,
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return cryptoChangeDto;
        }

        public async Task<IList<CryptoPriceLogDto>> GetCryptoChangesAsync(int cryptoId)
        {
            var changes = await _context.CryptoPriceLogs
                .Where(c => c.CryptoId == cryptoId)
                .Include(c => c.Crypto)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
            return _mapper.Map<IList<CryptoPriceLogDto>>(changes);
        }
    }
}
