using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data
{
    public static class Extentions
    {
        public static async Task<IApplicationBuilder> UseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<DiscountContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DiscountContext>>();

            try
            {
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migration applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying the database migration.");
            }

            return app;
        }
    }
}
