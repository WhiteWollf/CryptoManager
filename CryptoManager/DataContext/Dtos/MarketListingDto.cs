using DataContext.Entities;
using DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Dtos
{
    public class MarketListingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class MarketListingCreateDto
    {
        public int UserId { get; set; }
        public int CryptoId { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public EMarketType Type { get; set; }
    }

    public class AcceptMarketListingDto
    {
        public int UserId { get; set; }
    }
}
