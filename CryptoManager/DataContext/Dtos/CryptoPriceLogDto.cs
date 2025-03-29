using DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class CryptoPriceLogDto
    {
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
