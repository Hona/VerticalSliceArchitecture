using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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

    protected static readonly JsonSerializerOptions JsonSerializerOptions =
        new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    protected AsyncServiceScope NewScope() => _factory.Services.CreateAsyncScope();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        _factory = new TestAppFactory(_postgreSqlContainer.GetConnectionString());
        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions());

        await using var scope = NewScope();
        var db = scope.GetDbContext();
        await db.Database.EnsureCreatedAsync();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}
