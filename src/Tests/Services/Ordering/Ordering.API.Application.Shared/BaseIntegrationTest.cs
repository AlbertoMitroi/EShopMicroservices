using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data;
using Xunit;

namespace Ordering.Shared
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IServiceScope _scope;

        protected readonly ISender _Sender;
        protected readonly ApplicationDbContext _Context;
        protected readonly HttpClient _HttpClient;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();

            _Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

            _Context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _HttpClient = factory.CreateClient();
        }
    }
}
