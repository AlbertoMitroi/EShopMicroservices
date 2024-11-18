namespace Ordering.Domain.Abstractions
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken);
        Task<Order> GetByIdAsync(OrderId id, CancellationToken cancellationToken);
        Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken);
        Task<IEnumerable<Order>> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<Order>> GetPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<long> GetCountAsync(CancellationToken cancellationToken);
        Task AddAsync(Order order, CancellationToken cancellationToken);
        void Update(Order order);
        void Remove(Order order);
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
