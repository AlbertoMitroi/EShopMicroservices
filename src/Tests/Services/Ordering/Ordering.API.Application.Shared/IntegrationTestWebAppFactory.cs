using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data;
using Testcontainers.MsSql;
using Xunit;

namespace Ordering.Shared
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {

        private readonly MsSqlContainer _sqlContainer;
        public IntegrationTestWebAppFactory()
        {
            _sqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-CU15-GDR1-ubuntu-22.04")
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    var connectionString = _sqlContainer.GetConnectionString();
                    options.UseSqlServer(connectionString);
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _sqlContainer.StopAsync();
        }
    }
}