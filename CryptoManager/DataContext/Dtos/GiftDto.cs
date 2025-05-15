using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class GiftDto
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public int CryptoId { get; set; }
        public decimal Amount { get; set; }
    }
    public class GiftHistoryDto
    {
        public string Direction { get; set; } // "Sent" vagy "Received"
        public int CounterPartId { get; set; }
        public string CryptoName { get; set; }
        public decimal Amount { get; set; }
        public decimal PriceAtGiftTime { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
