namespace Basket.Api.Basket.GetBasket
{
    public record GetBasketResponse(ShoppingCart Cart);

    public class GetBasketEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {
                var result = await sender.Send(new GetBasketQuery(userName));

                var respose = result.Adapt<GetBasketResponse>();

                return Results.Ok(respose);
            })
            .WithName("GetUserBasket")
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User Basket")
            .WithDescription("Retrieves the shopping basket for the specified user.");
        }
    }
}
