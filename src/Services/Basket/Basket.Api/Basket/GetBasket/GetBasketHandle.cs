using Basket.Api.Models;
using BuildingBlocks.CQRS;

namespace Basket.Api.Basket.GetBasket
{
    public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
    public record GetBasketResult(ShoppingCart Cart);
    public class GetBasketQueryHandle
        ()
        : IQueryHandler<GetBasketQuery, GetBasketResult>
    {
        public async Task<GetBasketResult> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            return new GetBasketResult(new ShoppingCart("swn"));
        }
    }
}
