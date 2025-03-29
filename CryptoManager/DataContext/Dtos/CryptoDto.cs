using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class CryptoChangeDto
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public decimal Available { get; set; }
    }
}
