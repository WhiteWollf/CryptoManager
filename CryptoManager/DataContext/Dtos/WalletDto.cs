using DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class WalletDto
    {
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }
    public class WalletCryptoDto
    {
        public int WalletId { get; set; }
        public int CryptoId { get; set; }
        public decimal Amount { get; set; }
    }

    public class CryptoDto
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Available { get; set; }
    }

    public class WalletCryptoDetailDto
    {
        public string CryptoName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Amount { get; set; } // A user által birtokolt mennyiség
        public decimal UnitPrice { get; set; } // A kriptovaluta aktuális árfolyama
    }

    public class WalletDetailDto
    {
        public decimal Balance { get; set; }
        public List<WalletCryptoDetailDto> Cryptos { get; set; } = new();
    }
}
