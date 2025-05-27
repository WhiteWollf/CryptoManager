using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class CryptoInterestRate : AbstractEntity
    {
        public int CryptoId { get; set; }
        public Crypto Crypto { get; set; }
        public decimal InterestRate { get; set; } = 5;
    }
}
