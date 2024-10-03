using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace VerticalSliceArchitectureTemplate.Integration.Tests;

[DebuggerDisplay(
    $"{{{nameof(_postgreSqlContainer)}.{nameof(_postgreSqlContainer.GetMappedPublicPort)}(5432)}}"
)]
public class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    private TestAppFactory _factory = null!;

    protected HttpClient Client = null!;

    protected AsyncServiceScope NewScope() => _factory.Services.CreateAsyncScope();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        _factory = new TestAppFactory(_postgreSqlContainer.GetConnectionString());
        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}
