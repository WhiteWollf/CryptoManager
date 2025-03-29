using DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataContext.Context
{
    public class CryptoDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletCrypto> WalletCrypto { get; set; }
        public DbSet<Crypto> Cryptos { get; set; }
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options)
        {
            
        }
    }
}
