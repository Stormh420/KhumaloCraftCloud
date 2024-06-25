namespace KhumaloCraftPOE.Models.Entities
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public int OrderID { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
