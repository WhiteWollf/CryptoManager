using DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class MarketListing : AbstractEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int CryptoId {  get; set; }
        public Crypto Crypto { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public EMarketType Type { get; set; }
        public EMarketStatus Status { get; set; } = EMarketStatus.Active;
    }
}
