using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    [PrimaryKey(nameof(WalletId), nameof(CryptoId))]
    public class WalletCrypto
    {
        public int WalletId { get; set; }
        public int CryptoId { get; set; }
        public Crypto Crypto { get; set; }
        public decimal Amount { get; set; }
        public decimal LockedAmount { get; set; } = 0;
        public decimal BuyPrice { get; set; }
    }
}
