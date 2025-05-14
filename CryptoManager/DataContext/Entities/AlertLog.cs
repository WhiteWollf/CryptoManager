using DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class AlertLog : AbstractEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int CryptoId { get; set; }
        public Crypto Crypto { get; set; }
        public int TargetPrice { get; set; }
        public EAlertType AlertType { get; set; }
        public DateTime SuccesDateTime { get; set; } = DateTime.Now;
    }
}
