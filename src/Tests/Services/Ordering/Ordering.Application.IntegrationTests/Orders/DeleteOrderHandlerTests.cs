using FluentAssertions;
using Ordering.Application.Orders.Commands.DeleteOrder;
using Ordering.Shared;
using Xunit;

namespace Ordering.API.IntegrationTests.Orders
{
    public class DeleteOrderHandlerTests : BaseIntegrationTest
    {
        public DeleteOrderHandlerTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Handler_ShouldDelete_ExistingOrderWithGuid()
        {
            // Arrange 
            var postOrderResult = await HelperClass.PostNewOrder(_httpClient);

            // Act
            var result = await Sender.Send(new DeleteOrderCommand(postOrderResult));

            // Assert 
            result.IsSuccess.Should().Be(true);
        }
    }
}
