using Basket.Api.Basket.StoreBasket;
using Basket.Api.Data;
using Basket.Api.Models;
using Discount.Grpc;
using Grpc.Core;
using Moq;
using FluentAssertions;

namespace Basket.Api.UniTests.Basket.StoreBasket
{
    [TestClass]
    public class StoreBasketHandleTests
    {
        private Mock<IBasketRepository> _mockRepository;
        private Mock<DiscountProtoService.DiscountProtoServiceClient> _mockDiscountProto;
        private StoreBasketCommandHandle _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IBasketRepository>();
            _mockDiscountProto = new Mock<DiscountProtoService.DiscountProtoServiceClient>();

            _handler = new StoreBasketCommandHandle(_mockRepository.Object, _mockDiscountProto.Object);
        }

        public static IEnumerable<object[]> PriceAndDiscount
        {
            get
            {
                return new[]
                {
                    new object[] { new PriceDto(100, 20), 80 },
                };
            }
        }

        public record PriceDto(int price, int discountAmount);

        [TestMethod]
        [DynamicData(nameof(PriceAndDiscount))]
        public async Task DeductDiscount_DecreasePriceForCartItem_UpdatedPrice(PriceDto priceDto, int expectedPrice)
        {
            // Arrange

            var cart = new ShoppingCart
            {
                UserName = "Test",
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem { ProductName = "Product1", Price = priceDto.price }
                }
            };

            // mock discount service
            _mockDiscountProto
                .Setup(x => x.GetDiscountAsync(It.IsAny<GetDiscountRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<CouponModel>(
                    Task.FromResult(new CouponModel { Amount = priceDto.discountAmount }),   
                    Task.FromResult(new Metadata()), 
                    () => Status.DefaultSuccess,
                    () => new Metadata(),  
                    () => { }
                ));

            var command = new StoreBasketCommand(cart);

            // Act
            await _handler.Handle(command, default);

            // Assert 
            cart.Items[0].Price.Should().Be(expectedPrice);
        }

        [TestMethod]
        public async Task Handle_StoreBasketAndAppliesDiscount_CallsRepositoryWithUpdatedCart()
        {
            // Arrange
            var cart = new ShoppingCart
            {
                UserName = "Test",
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem { ProductName = "Product1", Price = 100 }
                }
            };
            var discountAmount = 15;

            // mock discount service
            _mockDiscountProto
                .Setup(x => x.GetDiscountAsync(It.IsAny<GetDiscountRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(new AsyncUnaryCall<CouponModel>(
                    Task.FromResult(new CouponModel { Amount = discountAmount }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
                ));

            // mock repository to do nothing
            _mockRepository
                .Setup(x => x.StoreBasket(It.IsAny<ShoppingCart>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var command = new StoreBasketCommand(cart);

            // Act
            await _handler.Handle(command, default);

            // Assert
            
            _mockRepository.Verify(x => x.StoreBasket(cart, It.IsAny<CancellationToken>()), Times.Once());
            cart.Items[0].Price.Should().Be(85);
        }
    }
}
