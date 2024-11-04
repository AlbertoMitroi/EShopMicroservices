namespace Basket.Api.Models
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public Guid ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;

        public void Deduct(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Deduction amount cannot be negative.", nameof(amount));

            Price -= amount;
        }
    }
}
