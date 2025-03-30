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
        public decimal Amount { get; set; }
        public decimal BuyPrice { get; set; } 
    }
    // A felhasználó pénztárcájának részletes adatait adja vissza, beleértve a pénztárcában található kriptovaluták listáját
    public class WalletDetailDto
    {
        public decimal Balance { get; set; }
        public List<WalletCryptoDetailDto> Cryptos { get; set; } = new();
    }
}
