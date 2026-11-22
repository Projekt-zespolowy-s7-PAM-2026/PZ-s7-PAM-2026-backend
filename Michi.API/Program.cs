using Michi.API.Exceptions;
using Michi.API.Identity.Extensions;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddIdentityModule(builder.Configuration);

var app = builder.Build();
await app.InitializeIdentityAsync();
app.UseExceptionHandler();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Michi API v1");
        options.RoutePrefix = "swagger";
    });
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready");

app.Run();
public partial class Program;