using Scalar.AspNetCore;
using Infrastructure;
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.Run();

