using FluentAssertions;
using Ordering.API.Endpoints;
using Ordering.Domain.ValueObjects;
using Ordering.Shared;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ordering.API.IntegrationTest
{
    public class DeleteOrderTests : BaseIntegrationTest
    {
        public DeleteOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturn200Ok_WhenOrderExists()
        {
            // Arrange
            Guid orderId = await HelperClass.PostNewOrder(_httpClient);

            // Act
            var deleteResponse = await _httpClient.DeleteAsync($"/orders/{orderId}");
            deleteResponse.EnsureSuccessStatusCode();

            var responseBody = await deleteResponse.Content.ReadFromJsonAsync<DeleteOrderResponse>();

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            responseBody.Should().NotBeNull();
            responseBody!.IsSuccess.Should().BeTrue();

            var deletedOrder = await _context.Orders.FindAsync(OrderId.Of(orderId));
            deletedOrder.Should().BeNull();
        }
        [Fact]
        public async Task DeleteOrder_ShouldReturn404NotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            Guid nonExistentOrderId = Guid.NewGuid();

            // Act
            var deleteResponse = await _httpClient.DeleteAsync($"/orders/{nonExistentOrderId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturn400BadRequest_WhenIdIsInvalid()
        {
            // Arrange
            Guid invalidOrderId = Guid.NewGuid();

            // Act
            var deleteResponse = await _httpClient.DeleteAsync($"/orders/{invalidOrderId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}