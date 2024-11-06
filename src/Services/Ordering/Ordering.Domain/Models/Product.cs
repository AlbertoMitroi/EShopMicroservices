namespace Ordering.Domain.Models
{
    public class Product : Entity<ProductId>
    { 
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
    }
}
