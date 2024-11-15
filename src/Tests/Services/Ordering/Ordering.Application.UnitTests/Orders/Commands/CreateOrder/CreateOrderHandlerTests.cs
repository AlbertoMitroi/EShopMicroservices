using FluentAssertions;
using Mapster;
using Moq;
using Ordering.Application.Data;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Models;
using Ordering.Shared;

namespace Ordering.Application.UnitTests.Orders.Commands.CreateOrder
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly CreateOrderHandler _handler;

        public CreateOrderHandlerTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _handler = new CreateOrderHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidOrder_CreatesOrderAndReturnsOrderId()
        {
            // Arrange
            var orderDto = HelperClass.GetValidOrderDto();
            var command = new CreateOrderCommand(orderDto);

            _mockRepository.Setup(repo => repo.AddAsync(It.Is<Order>(o => o.OrderName.Value == orderDto.OrderName), default))
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(repo => repo.SaveAsync(default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CreateOrderResult>();
            _mockRepository.Verify(repo => repo.AddAsync(It.Is<Order>(o => o.OrderName.Value == orderDto.OrderName), default), Times.Once());
            _mockRepository.Verify(repo => repo.SaveAsync(default), Times.Once());
        }
    }
}
