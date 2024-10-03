using Microsoft.Extensions.DependencyInjection;
using VerticalSliceArchitectureTemplate.Common.EfCore;

namespace VerticalSliceArchitectureTemplate.Integration.Tests;

public static class ScopeExtensions
{
    public static AppDbContext GetDbContext(this AsyncServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<AppDbContext>();
}
