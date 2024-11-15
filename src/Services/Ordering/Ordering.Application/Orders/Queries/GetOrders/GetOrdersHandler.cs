using BuildingBlocks.Pagination;
using Ordering.Domain.Abstractions;

namespace Ordering.Application.Orders.Queries.GetOrders
{
    public class GetOrdersHandler(IOrderRepository repository)
    : IQueryHandler<GetOrdersQuery, GetOrdersResult>
    {
        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCount = await repository.GetCountAsync(cancellationToken);

            var orders = await repository.GetPageAsync(pageIndex, pageSize, cancellationToken);

            return new GetOrdersResult(
                new PaginatedResult<OrderDto>(
                    pageIndex,
                    pageSize,
                    totalCount,
                    orders.ToOrderDtoList()));
        }
    }
}
