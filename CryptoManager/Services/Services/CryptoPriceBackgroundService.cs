using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CryptoPriceBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _random = new();

        public CryptoPriceBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateCryptoPrices(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }

        private async Task UpdateCryptoPrices(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();

                var cryptos = await _context.Cryptos.ToListAsync(stoppingToken);
                if (!cryptos.Any()) return;

                foreach (var crypto in cryptos)
                {
                    decimal changePercentage = (decimal)(_random.NextDouble() * 0.1 - 0.05); // ±5%
                    decimal newPrice = Math.Round(crypto.Price * (1 + changePercentage), 2);

                    _context.CryptoPriceLogs.Add(new CryptoPriceLog
                    {
                        CryptoId = crypto.Id,
                        OldPrice = crypto.Price,
                        NewPrice = newPrice,
                        Date = DateTime.Now
                    });

                    crypto.Price = newPrice;
                }

                await _context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
