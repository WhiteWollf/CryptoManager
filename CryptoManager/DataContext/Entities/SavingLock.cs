using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class SavingLock : AbstractEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int CryptoId {  get; set; }
        public Crypto Crypto { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
