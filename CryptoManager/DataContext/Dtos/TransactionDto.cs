using DataContext.Entities;

namespace Services.Services
{
    public class TransactionDto
    {
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class DetailedTransactionDto
    {
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal CurrentUnitPrice { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}