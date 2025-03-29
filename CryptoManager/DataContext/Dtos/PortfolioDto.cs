using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos 
{ 

    public class PortfolioDetailDto
    {
        public string CryptoName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Amount { get; set; } // A user által birtokolt mennyiség
        public decimal UnitPrice { get; set; } // A kriptovaluta aktuális árfolyama
        public decimal Value { get; set; } //A felhasználó által birtokolt kriptovaluta értéke
    }
    public class PortfolioDto
    {
        public decimal TotalValue { get; set; }
        public decimal Balance { get; set; }
        public List<PortfolioDetailDto> Cryptos { get; set; } = new();
    }

}