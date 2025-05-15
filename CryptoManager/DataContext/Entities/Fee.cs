using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class TransactionFee : AbstractEntity
    {
        public decimal Fee { get; set; }
    }
}
