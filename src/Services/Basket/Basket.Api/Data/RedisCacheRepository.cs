using StackExchange.Redis;

namespace Basket.Api.Data
{
    public class RedisCacheRepository(IBasketRepository repository, IConnectionMultiplexer connectionMultiplexer) : IBasketRepository
    {
        private readonly IDatabase cache = connectionMultiplexer.GetDatabase();

        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
        {
            var cachedBasket = await cache.StringGetAsync(userName);

            if (!string.IsNullOrEmpty(cachedBasket))
            {
                var basket = JsonSerializer.Deserialize<ShoppingCart>(cachedBasket);
                if (basket != null)
                    return basket;
            }

            var fetchedBasket = await repository.GetBasket(userName, cancellationToken);

            await cache.StringSetAsync(userName, JsonSerializer.Serialize(fetchedBasket));

            return fetchedBasket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            await repository.StoreBasket(basket, cancellationToken);

            await cache.StringSetAsync(basket.UserName, JsonSerializer.Serialize(basket));

            return basket;
        }

        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            await repository.DeleteBasket(userName, cancellationToken);

            await cache.KeyDeleteAsync(userName);

            return true;
        }
    }
}
