using System.Reflection;

var appAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

// Host
builder.Services.AddSwaggerGen( options =>
{
    options.CustomSchemaIds(x => x.FullName?.Replace("+", ".", StringComparison.Ordinal));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();