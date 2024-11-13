using FluentAssertions;
using Moq;
using Ordering.Application.Data;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Models;

namespace Ordering.Application.UnitTests.Orders.Commands.CreateOrder
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockDbContext;
        private readonly CreateOrderHandler _handler;

        public CreateOrderHandlerTests()
        {
            _mockDbContext = new Mock<IApplicationDbContext>();
            _handler = new CreateOrderHandler(_mockDbContext.Object);
        }

        [Fact]
        public async Task Handle_WithValidOrder_CreatesOrderAndReturnsOrderId()
        {
            // Arrange
            var orderDto = HelperMethods.GetValidOrderDto();
            var command = new CreateOrderCommand(orderDto);

            _mockDbContext.Setup(db => db.Orders.Add(It.IsAny<Order>()));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CreateOrderResult>();
            _mockDbContext.Verify(db => db.Orders.Add(It.IsAny<Order>()), Times.Once());
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
