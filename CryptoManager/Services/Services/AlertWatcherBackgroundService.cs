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
    public class AlertWatcherBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AlertWatcherBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await WatchAlerts(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task WatchAlerts(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();

                var alerts = await _context.Alerts.ToListAsync(stoppingToken);
                if (!alerts.Any()) return;

                foreach (var alert in alerts)
                {
                    var crypto = await _context.Cryptos.FirstOrDefaultAsync(a => a.Id == alert.CryptoId, stoppingToken);
                    if (crypto == default)
                    {
                        continue;
                    }
                    if((alert.AlertType == EAlertType.Above && alert.TargetPrice<=crypto.Price) || (alert.AlertType == EAlertType.Below && alert.TargetPrice >= crypto.Price))
                    {
                        var alertLog = new AlertLog()
                        {
                            CryptoId = alert.CryptoId,
                            AlertType = alert.AlertType,
                            UserId = alert.UserId,
                            TargetPrice = alert.TargetPrice
                        };
                        await _context.AlertLogs.AddAsync(alertLog, stoppingToken);
                        _context.Alerts.Remove(alert);
                        await _context.SaveChangesAsync(stoppingToken);
                    }
                }

                await _context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
