using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class Crypto : AbstractEntity
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Available { get; set; } //Elérhető mennyiség
    }
}
