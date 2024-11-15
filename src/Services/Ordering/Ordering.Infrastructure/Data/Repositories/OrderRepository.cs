using BuildingBlocks.Exceptions;

namespace Ordering.Infrastructure.Data.Repositories
{
    public class OrderRepository(ApplicationDbContext _context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Order> orders = await _context.Orders.ToListAsync(cancellationToken);

            return orders;
        }

        public async Task<Order> GetByIdAsync(OrderId id, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(id, cancellationToken);

            if (order == null)
                throw new NotFoundException($"Order with id: {id} was not found in repository");


            return order;
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                            .Include(o => o.OrderItems)
                            .AsNoTracking()
                            .Where(o => o.CustomerId == CustomerId.Of(customerId))
                            .OrderBy(o => o.OrderName.Value)
                            .ToListAsync(cancellationToken);

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                            .Include(o => o.OrderItems)
                            .AsNoTracking()
                            .Where(o => o.OrderName.Value.Contains(name))
                            .OrderBy(o => o.OrderName.Value)
                            .ToListAsync(cancellationToken);

            return orders;
        }

        public async Task<IEnumerable<Order>> GetPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<Order> orders = await _context.Orders
                            .Include(o => o.OrderItems)
                            .OrderBy(o => o.OrderName.Value)
                            .Skip(pageSize * pageIndex)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

            return orders;
        }

        public Task<long> GetCountAsync(CancellationToken cancellationToken)
        {
            var count = _context.Orders.LongCountAsync(cancellationToken);

            return count;
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
