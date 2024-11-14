using FluentAssertions;
using Ordering.API.Endpoints;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Enums;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Ordering.Shared
{
    public static class HelperClass
    {

        public static OrderDto GetValidOrderDto()
        {

            var orderId = Guid.NewGuid();

            var customerId = Guid.Parse("58c49479-ec65-4de2-86e7-033c546291aa");

            var orderItemDtoId = Guid.NewGuid();

            var productId1 = Guid.Parse("5334c996-8457-4cf0-815c-ed2b77c4ff61");
            var productId2 = Guid.Parse("5334c996-8457-4cf0-815c-ed2b77c4ff61");

            // Create dto
            var orderDto = new OrderDto(
                Id: orderId,
                CustomerId: customerId,
                OrderName: "ORD_TEST",
                ShippingAddress: new AddressDto(
                    FirstName: "First_TEST",
                    LastName: "Last_TEST",
                    EmailAddress: "TEST@email.com",
                    AddressLine: "Address_TEST",
                    Country: "USA_TEST",
                    State: "STATE_TEST",
                    ZipCode: "12345"
                ),
                BillingAddress: new AddressDto(
                    FirstName: "First_TEST",
                    LastName: "Last_TEST",
                    EmailAddress: "TEST@email.com",
                    AddressLine: "Address_TEST",
                    Country: "USA_TEST",
                    State: "STATE_TEST",
                    ZipCode: "12345"
                ),
                Payment: new PaymentDto(
                    CardName: "CARD_TEST",
                    CardNumber: "1111111111111111",
                    Expiration: "11/33",
                    Cvv: "123",
                    PaymentMethod: 1
                ),
                Status: OrderStatus.Draft,
                OrderItems: new List<OrderItemDto>
                {
                    new OrderItemDto(
                            OrderId: orderItemDtoId,
                            ProductId: productId1,
                            Quantity: 2,
                            Price: 500
                            ),
                    new OrderItemDto(
                            OrderId: orderItemDtoId,
                            ProductId: productId2,
                            Quantity: 1,
                            Price: 500
                            )
                }
            );

            return orderDto;
        }

        public static Order GetValidOrder()
        {
            var orderDto = GetValidOrderDto();

            var order = Order.Create(
                OrderId.Of(orderDto.Id),
                CustomerId.Of(orderDto.CustomerId),
                OrderName.Of(orderDto.OrderName),
                 Address.Of(
                    orderDto.ShippingAddress.FirstName,
                    orderDto.ShippingAddress.LastName,
                    orderDto.ShippingAddress.EmailAddress,
                    orderDto.ShippingAddress.AddressLine,
                    orderDto.ShippingAddress.Country,
                    orderDto.ShippingAddress.State,
                    orderDto.ShippingAddress.ZipCode
                ),
                 Address.Of(
                    orderDto.BillingAddress.FirstName,
                    orderDto.BillingAddress.LastName,
                    orderDto.BillingAddress.EmailAddress,
                    orderDto.BillingAddress.AddressLine,
                    orderDto.BillingAddress.Country,
                    orderDto.BillingAddress.State,
                    orderDto.BillingAddress.ZipCode
                ),
                Payment.Of(
                    orderDto.Payment.CardName,
                    orderDto.Payment.CardNumber,
                    orderDto.Payment.Expiration,
                    orderDto.Payment.Cvv,
                    orderDto.Payment.PaymentMethod
                )
            );

            foreach (var itemDto in orderDto.OrderItems)
            {
                order.Add(
                     ProductId.Of(itemDto.ProductId),
                    itemDto.Quantity,
                    itemDto.Price
                );
            }

            return order;
        }

        public static async Task<Guid> PostNewOrder(HttpClient httpClient)
        {
            var createOrderRequest = CreateNewOrderRequest();

            var createResponse = await httpClient.PostAsJsonAsync<CreateOrderRequest>("/orders", createOrderRequest);
            createResponse.EnsureSuccessStatusCode();

            var createOrderResponse = await createResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var createdOrder = JsonSerializer.Deserialize<CreateOrderResponse>(createOrderResponse, options);

            createdOrder.Should().NotBeNull();
            createdOrder!.Id.Should().NotBeEmpty();

            return createdOrder.Id;
        }

        public static CreateOrderRequest CreateNewOrderRequest()
        {
            var orderDto = GetValidOrderDto();
            return new CreateOrderRequest(orderDto);
        }
    }
}
