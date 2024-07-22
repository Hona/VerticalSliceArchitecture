using System.Text.Json.Serialization;
using Mapster;
using VerticalSliceArchitectureTemplate;
using VerticalSliceArchitectureTemplate.Common.EfCore;

// Allow Strong IDs to generate nice OpenAPI schemas
[assembly: VogenDefaults(
    openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod
)]

var builder = WebApplication.CreateBuilder(args);

TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly); // Wire up Mapster to scan the assembly for IRegister implementations

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName?.Replace("+", ".", StringComparison.Ordinal)); // Support Request/Response objects being a child of the Use Case for the JSON Schema
    options.MapVogenTypes(); // Make sure that Vogen types are generating the underlying type
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddAppDbContext(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Enums as strings over the wire
    options.SerializerOptions.Converters.Add(new VogenTypesFactory()); // Convert all Vogen value objects to their correct types
});

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

/*
#if DEBUG
using (var dbScope = app.Services.CreateScope())
{
    var db = dbScope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}
#endif
*/

app.Run();
