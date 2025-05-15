using DataContext.Entities;

namespace Services.Services
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public String Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
    }

    public class DetailedTransactionDto
    {
        public string CryptoName { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal CurrentUnitPrice { get; set; }
        public decimal CurrentTotalPrice => Amount * CurrentUnitPrice;
        public String Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
    }

    public class TradeRecepit
    {
        public int transactionId { get; set; }
        public int userId { get; set; }
        public int cryptoId { get; set; }
        public decimal amount { get; set; }
        public decimal fee { get; set; }
        public decimal totalAmount { get; set; }
        public String TimeStamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
    }
}