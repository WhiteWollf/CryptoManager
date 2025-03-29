using DataContext.Context;
using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class InitializerService : IHostedService
    {
        private readonly CryptoDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        public InitializerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();

                var cryptos = new List<Crypto>
                {
                    new Crypto { Name = "Bitcoin", Symbol = "BTC", Price = 1000000, Available = 1000000 },
                    new Crypto { Name = "Ethereum", Symbol = "ETH", Price = 500000, Available = 500000 },
                    new Crypto { Name = "PepeCoin ", Symbol = "PEPE", Price = 250000, Available = 250000 },
                    new Crypto { Name = "Dogecoin", Symbol = "DOGE", Price = 100000, Available = 100000 },
                    new Crypto { Name = "Shiba Inu", Symbol = "SHIB", Price = 50000, Available = 50000 },
                    new Crypto { Name = "Dotnet", Symbol = ".NET", Price = 25000, Available = 25000 },
                    new Crypto { Name = "TenThousandcoin", Symbol = "TENK", Price = 10000, Available = 10000 },
                    new Crypto { Name = "Thousandcoin", Symbol = "1000", Price = 4321, Available = 1000 },
                    new Crypto { Name = "FiveHundredcoin", Symbol = "FIVEH", Price = 500, Available = 500 },
                    new Crypto { Name = "TwoHundredcoin", Symbol = "TWOH", Price = 200, Available = 200 },
                    new Crypto { Name = "Hundredcoin", Symbol = "HUND", Price = 100, Available = 100 },
                    new Crypto { Name = "Fiftycoin", Symbol = "FIFTY", Price = 50, Available = 50 },
                    new Crypto { Name = "Tencoin", Symbol = "TEN", Price = 10, Available = 10 },
                    new Crypto { Name = "Fivecoin", Symbol = "FIVE", Price = 5, Available = 5 },
                    new Crypto { Name = "Onecoin", Symbol = "ONE", Price = 1, Available = 1 }

                };
                foreach (var crypto in cryptos)
                {
                    var existingCrypto = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == crypto.Symbol, cancellationToken);
                    if (existingCrypto == null)
                    {
                        _context.Cryptos.Add(crypto);
                    }
                    else
                    {
                        existingCrypto.Price = crypto.Price;
                        existingCrypto.Available = crypto.Available;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                };

                foreach (var role in roles)
                {
                    var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role.Name, cancellationToken);
                    if (existingRole == null)
                    {
                        _context.Roles.Add(role);
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);

                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin", cancellationToken);
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);

                var users = new List<User>
                {
                    new User { Name = "admin", Email = "admin@example.com", PasswordHash=BCrypt.Net.BCrypt.HashPassword("admin"), Roles= new List<Role> { adminRole, userRole } },
                    new User { Name = "user", Email = "user@example.com", PasswordHash=BCrypt.Net.BCrypt.HashPassword("user"), Roles=new List<Role> { userRole } }
                };

                foreach (var user in users)
                {
                    var existingUser = await _context.Users.Include(u=>u.Roles).FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);
                    if (existingUser == null)
                    {
                        _context.Users.Add(user);
                    }
                    else
                    {
                        existingUser.Name = user.Name;
                        existingUser.PasswordHash = user.PasswordHash;
                        existingUser.Roles = user.Roles;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);

                var usersWithoutWallet = await _context.Users
                .Where(u => !_context.Wallets.Any(w => w.UserId == u.Id))
                .ToListAsync(cancellationToken);

                if (usersWithoutWallet.Any())
                {
                    var wallets = usersWithoutWallet.Select(user => new Wallet
                    {
                        UserId = user.Id,
                        Balance = 10000
                    }).ToList();

                    _context.Wallets.AddRange(wallets);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                Crypto crypto1 = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == "BTC", cancellationToken);
                Crypto crypto2 = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == "TEN", cancellationToken);
                Crypto crypto3 = await _context.Cryptos.FirstOrDefaultAsync(c => c.Symbol == "FIVE", cancellationToken);
                User admin = await _context.Users.FirstOrDefaultAsync(u=>u.Name == "admin", cancellationToken);
                Wallet adminWallett = await _context.Wallets.FirstOrDefaultAsync(w=>w.UserId==admin.Id, cancellationToken);

                _context.WalletCrypto.RemoveRange(await _context.WalletCrypto.Where(a => a.WalletId == adminWallett.Id).ToListAsync(cancellationToken));
                await _context.SaveChangesAsync(cancellationToken);
                List<WalletCrypto> walletCrypto = new List<WalletCrypto>()
                {
                    new WalletCrypto { CryptoId = crypto1.Id, WalletId = adminWallett.Id, Amount = 500 },
                    new WalletCrypto { CryptoId = crypto2.Id, WalletId = adminWallett.Id, Amount = 5 },
                    new WalletCrypto { CryptoId = crypto3.Id, WalletId = adminWallett.Id, Amount = 2 }
                };
                crypto1.Available -= 500;
                crypto2.Available -= 5;
                crypto3.Available -= 2;
                _context.WalletCrypto.AddRange(walletCrypto);

                await _context.SaveChangesAsync(cancellationToken);
            }
            return;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
