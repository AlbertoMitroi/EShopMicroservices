using Ordering.Application.Dtos;
using Ordering.Domain.Enums;

namespace Ordering.Application.UnitTests.Orders
{
    public static class HelperMethods
    {
        public static OrderDto GetValidOrderDto()
        {
            var orderDto = new OrderDto(
                Id: Guid.NewGuid(),
                CustomerId: Guid.NewGuid(),
                OrderName: "TestName",
                ShippingAddress: new AddressDto(
                    FirstName: "TestFirst",
                    LastName: "TestLast",
                    EmailAddress: "test@test.com",
                    AddressLine: "test 112",
                    Country: "IT",
                    State: "IT",
                    ZipCode: "12345"
                ),
                BillingAddress: new AddressDto(
                    FirstName: "TestFirst",
                    LastName: "TestLast",
                    EmailAddress: "test@test.com",
                    AddressLine: "test 112",
                    Country: "IT",
                    State: "IT",
                    ZipCode: "12345"
                ),
                Payment: new PaymentDto(
                    CardName: "test test",
                    CardNumber: "1111111111111111",
                    Expiration: "12/25",
                    Cvv: "123",
                    PaymentMethod: 1
                ),
                Status: OrderStatus.Draft,
                OrderItems: new List<OrderItemDto>
                {
                    new OrderItemDto(
                            OrderId: Guid.NewGuid(),
                            ProductId: Guid.NewGuid(),
                            Quantity: 2,
                            Price: 100)
                }
            );

            return orderDto;
        }
    }
}
