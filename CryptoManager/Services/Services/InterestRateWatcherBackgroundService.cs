using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using DataContext.Enums;
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
    public class DailyInterestBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _runTime = TimeSpan.FromHours(2); // Pl. minden nap hajnal 2-kor

        public DailyInterestBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await WatchSavings(stoppingToken);
                //await Task.Delay(TimeSpan.FromDays(1), stoppingToken); //Rendesen naponta nézze
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); //Láthatóságért most egy 30 mp úgymond egy nap :)
                //De nem látszódik az autómatikus lezárás így, mert az idő attól még nem változik, hogy elérje az enddate-t
            }
        }


        protected async Task WatchSavings(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();

                var activeLocks = await context.SavingLocks
                    .Where(s => s.IsActive)
                    .ToListAsync(stoppingToken);

                foreach (var lockItem in activeLocks)
                {
                    decimal dailyInterestFactor = 1 + (lockItem.InterestRate / 100);
                    lockItem.TotalAmount = lockItem.TotalAmount * (1 + lockItem.InterestRate / 100);
                    if (lockItem.EndDate.Date <= DateTime.UtcNow.Date)
                    {
                        lockItem.IsActive = false;
                        var userwallett = await context.Wallets.FirstOrDefaultAsync(u => u.UserId == lockItem.UserId);
                        var userwalettcrypto = await context.WalletCrypto.FirstOrDefaultAsync(u => u.WalletId == userwallett.Id && u.CryptoId==lockItem.CryptoId);
                        if (userwalettcrypto != null)
                        {
                            var earnedAmount = lockItem.TotalAmount - lockItem.Amount;
                            userwalettcrypto.Amount += earnedAmount * (decimal)0.85; // -15%
                            userwalettcrypto.LockedAmount -= lockItem.Amount;
                        }
                        else
                        {
                            Console.WriteLine("Hiba InterestRateWatcherBackgroundService: Nem található a saving crypto a felhasználónál");//elméletileg nem fordulhat elő
                        }
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba InterestRateWatcherBackgroundService: {ex}");
            }

        }
    }

}
