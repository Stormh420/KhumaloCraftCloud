namespace KhumaloCraftPOE.Models.Entities
{
    public class Cart
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation property
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
