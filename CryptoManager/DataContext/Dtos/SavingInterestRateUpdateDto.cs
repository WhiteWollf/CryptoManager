using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class CryptoInterestRateDto
    {
        public int CryptoId { get; set; }
        public string CryptoName { get; set; }
        public decimal InterestRate { get; set; }
    }
    public class CryptoInterestRateUpdateDto
    {
        public int CryptoId { get; set; }
        public decimal NewInterestRate { get; set; }
    }
}
