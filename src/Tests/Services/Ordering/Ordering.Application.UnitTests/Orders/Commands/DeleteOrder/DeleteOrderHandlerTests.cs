using FluentAssertions;
using Mapster;
using Moq;
using Ordering.Application.Orders.Commands.DeleteOrder;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;
using Ordering.Shared;

namespace Ordering.Application.UnitTests.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandlerTests 
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly DeleteOrderHandler _handle;

        public DeleteOrderHandlerTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _handle = new DeleteOrderHandler(_mockRepository.Object);
        }
        [Fact]
        public async Task Handle_WithValidOrder_DeletesOrderAndReturnsTrue()
        {
            // Arrange
            var order = HelperClass.GetValidOrder();
            _mockRepository.Setup(repo => repo.GetByIdAsync(OrderId.Of(order.Id.Value), default))
                .ReturnsAsync(order);

            // Act
            var result = await _handle.Handle(new DeleteOrderCommand(order.Id.Value), default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(repo => repo.GetByIdAsync(OrderId.Of(order.Id.Value), default), Times.Once);
            _mockRepository.Verify(repo => repo.Remove(order), Times.Once);
            _mockRepository.Verify(repo => repo.SaveAsync(default), Times.Once);
        }

    }
}
