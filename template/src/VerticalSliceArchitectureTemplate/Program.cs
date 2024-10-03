using System.Text.Json.Serialization;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

[assembly: VogenDefaults(customizations: Customizations.AddFactoryMethodForGuids)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints(options =>
{
    options.SourceGeneratorDiscoveredTypes.AddRange(
        VerticalSliceArchitectureTemplate.DiscoveredTypes.All
    );
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.SwaggerDocument();

builder.Services.AddAppDbContext(builder.Configuration);

var app = builder.Build();

app.UseFastEndpoints(config =>
{
    config.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});
app.UseSwaggerGen();

#if DEBUG
using (var dbScope = app.Services.CreateScope())
{
    var db = dbScope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await db.Database.MigrateAsync();
}
#endif

app.Run();

public partial class Program;
