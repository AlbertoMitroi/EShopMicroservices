using Ordering.Domain.Abstractions;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler(IOrderRepository repository)
    : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderId = OrderId.Of(command.Order.Id);
            var order = await repository.GetByIdAsync(orderId, cancellationToken);

            if (order is null)
            {
                throw new OrderNotFoundException(command.Order.Id);
            }

            UpdateOrderWithNewValues(order, command.Order);

            repository.Update(order);
            await repository.SaveAsync(cancellationToken);

            return new UpdateOrderResult(true);
        }

        public void UpdateOrderWithNewValues(Order order, OrderDto orderDto)
        {
            var updatedShippingAddress = Address.Of(
                orderDto.ShippingAddress.FirstName, 
                orderDto.ShippingAddress.LastName, 
                orderDto.ShippingAddress.EmailAddress, 
                orderDto.ShippingAddress.AddressLine, 
                orderDto.ShippingAddress.Country, 
                orderDto.ShippingAddress.State, 
                orderDto.ShippingAddress.ZipCode);
            var updatedBillingAddress = Address.Of(
                orderDto.BillingAddress.FirstName,
                orderDto.BillingAddress.LastName,
                orderDto.BillingAddress.EmailAddress,
                orderDto.BillingAddress.AddressLine,
                orderDto.BillingAddress.Country,
                orderDto.BillingAddress.State,
                orderDto.BillingAddress.ZipCode);
            var updatedPayment = Payment.Of(
                orderDto.Payment.CardName,
                orderDto.Payment.CardNumber, 
                orderDto.Payment.Expiration,
                orderDto.Payment.Cvv,
                orderDto.Payment.PaymentMethod);

            order.Update(
                orderName: OrderName.Of(orderDto.OrderName),
                shippingAddress: updatedShippingAddress,
                billingAddress: updatedBillingAddress,
                payment: updatedPayment,
                status: orderDto.Status);
        }
    }
}
