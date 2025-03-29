﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.Entities
{
    public class TransactionLog : AbstractEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int CryptoId { get; set; }
        public Crypto Crypto { get; set; }
        public string? Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice => PricePerUnit * Amount;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public TransactionType Type { get; set; }
    }

    public enum TransactionType
    {
        Buy,
        Sell
    }
}
