using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class TotalProfitLossDto
    {
        public decimal TotalProfitLoss { get; set; }
        public decimal TotalProfitLossPercentage { get; set; }
    }


    public class CryptoProfitLossDto
    {
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public decimal BuyValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitLossPercentage { get; set; }
    }

}
