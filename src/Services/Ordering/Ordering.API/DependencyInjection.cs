using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Exceptions.Middleware;
using Carter;

namespace Ordering.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddCarter();

            services.AddSingleton<CustomExceptionHandler>(); // instead of services.AddExceptionHandler<CustomExceptionHandler>(); 

            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)
        {
            app.MapCarter();

            app.UseMiddleware<CustomExceptionMiddleware>(); // instead of app.UseExceptionHandler(options => { });

            return app;
        }
    }
}
