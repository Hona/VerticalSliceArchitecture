using System.Text.Json.Serialization;
using VerticalSliceArchitectureTemplate.Common.EfCore;
using VerticalSliceArchitectureTemplate.Domain;

var builder = WebApplication.CreateBuilder(args);

// Support Request/Response objects being a child of the Use Case for the JSON Schema
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName?.Replace("+", ".", StringComparison.Ordinal));
    options.MapVogenTypes();
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddAppDbContext(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new GameId.GameIdSystemTextJsonConverter());
});

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();
