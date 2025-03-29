using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class CryptoPriceLog : AbstractEntity
    {
        public int CryptoId{ get; set; }
        public Crypto Crypto { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
