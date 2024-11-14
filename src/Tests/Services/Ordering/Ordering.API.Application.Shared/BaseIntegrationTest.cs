using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data;
using Xunit;

namespace Ordering.Shared
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IServiceScope _scope;
        protected readonly ISender Sender;
        protected readonly ApplicationDbContext _context;
        private readonly IntegrationTestWebAppFactory factory;
        protected readonly HttpClient _httpClient;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _httpClient = factory.CreateClient();

            this.factory = factory;
        }

        protected IServiceScope CreateScope()
        {
            return factory.Services.CreateScope();
        }
    }
}
