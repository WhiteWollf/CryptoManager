using DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class GiftListing : AbstractEntity
    {
        public int SenderUserId { get; set; }
        //public User SenderUser { get; set; }
        public int RecieverUserId { get; set; }
        //public User RecieverUser { get; set; }
        public int CryptoId {  get; set; }
        public Crypto Crypto { get; set; }
        public decimal Amount { get; set; }
    }
}
