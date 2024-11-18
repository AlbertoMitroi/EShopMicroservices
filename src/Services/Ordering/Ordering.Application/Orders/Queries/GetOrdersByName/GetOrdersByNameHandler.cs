using Ordering.Domain.Abstractions;

namespace Ordering.Application.Orders.Queries.GetOrdersByName
{
    public class GetOrdersByNameHandler(IOrderRepository repository)
        : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
    {
        public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
        {
            var orders = await repository.GetByNameAsync(query.Name, cancellationToken);

            return new GetOrdersByNameResult(orders.ToOrderDtoList());
        }
    }
}
