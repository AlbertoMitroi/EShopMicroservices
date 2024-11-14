using Mapster;
using MediatR;
using Moq;
using Ordering.Application.Data;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.DeleteOrder;
using Ordering.Domain.Models;
using Ordering.Shared;

namespace Ordering.Application.UnitTests.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandlerTests 
    {
        private readonly Mock<IApplicationDbContext> _mockDbContext;
        private readonly DeleteOrderHandler _handle;

        public DeleteOrderHandlerTests()
        {
            _mockDbContext = new Mock<IApplicationDbContext>();
            _handle = new DeleteOrderHandler(_mockDbContext.Object);
        }
        [Fact]
        public async Task Handle_WithValidOrder_DeletesOrderAndReturnsTrue()
        {
            // Arrange
            Order order = HelperClass.GetValidOrder();
            _mockDbContext.Setup(db =>
                db.Orders.FindAsync(new object[] { order.Id.Value }, default))
                .ReturnsAsync(order);

            var handler = new DeleteOrderHandler(_mockDbContext.Object);
            // Act
            var result = await handler.Handle(new DeleteOrderCommand(order.Id.Value), default);

            // Assert
            Assert.True(result.Adapt<bool>());
            _mockDbContext.Verify(db => db.Orders.Remove(order.Adapt<Order>()), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

    }
}
