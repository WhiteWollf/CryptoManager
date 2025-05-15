using DataContext.Entities;

namespace Services.Services
{
    public class TransactionFeeDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptoId { get; set; }
        public string Description { get; set; }
        public string CryptoName { get; set; }
        public string Type { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Amount { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FeePrice { get; set; }
        public String Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
    }

    public class TransactionWithFeeDto
    {
        public int TransactionId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoName { get; set; }
        public decimal FeePrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class FeeSummaryDto
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<TransactionWithFeeDto> Transactions { get; set; }
    }

}