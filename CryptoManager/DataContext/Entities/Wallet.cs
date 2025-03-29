using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class Wallet : AbstractEntity
    {
        public int UserId { get; set; } 
        public decimal Balance { get; set; }
        public List<WalletCrypto> Cryptos { get; set; }
    }
}
