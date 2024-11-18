using Ordering.Domain.Abstractions;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler(IOrderRepository repository)
    : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
    {
        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var orderId = OrderId.Of(command.OrderId);

            var order = await repository.GetByIdAsync(orderId, cancellationToken);

            if (order is null)
            {
                throw new OrderNotFoundException(command.OrderId);
            }

            repository.Remove(order);
            await repository.SaveAsync(cancellationToken);

            return new DeleteOrderResult(true);
        }
    }
}
