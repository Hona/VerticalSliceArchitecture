using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints(options =>
{
    options.SourceGeneratorDiscoveredTypes.AddRange(
        VerticalSliceArchitectureTemplate.DiscoveredTypes.All
    );
});
builder.Services.SwaggerDocument();

builder.Services.AddAppDbContext(builder.Configuration);

var app = builder.Build();

app.UseFastEndpoints();
app.UseSwaggerGen();

#if DEBUG
using (var dbScope = app.Services.CreateScope())
{
    var db = dbScope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}
#endif

app.Run();
