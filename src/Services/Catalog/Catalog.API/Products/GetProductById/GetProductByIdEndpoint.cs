

namespace Catalog.API.Products.GetProductById
{
    public class GetProductByIdEndpoint : ICarterModule
    {
        //public record GetProductByIdRequest();
        public record GetProductByIdResponse(Product Product);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));

                var respone = result.Adapt<GetProductByIdResponse>();

                return Results.Ok(respone);
            })
            .WithName("GetProductById")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id")
            .WithDescription("Get Product By Id");
        }
    }
}
