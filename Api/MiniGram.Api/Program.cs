using MiniGram.Api.Configuration;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var services = builder.Services;

services
    .AddOptions<AppSettings>().Bind(builder.Configuration);

services
    .AddOpenApi()
    .AddApplicationServices(builder.Configuration)
    .AddJwtBearer(builder.Configuration)
    .AddAuthorization()
    ;

var app = builder.Build();
app.UseCors();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.RegisterRoutes();

try
{
    Log.Information("MiniGramApi is starting");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "MiniGramApi terminated unexpectedly");
    return -1;
}
finally
{
    Log.CloseAndFlush();
}


namespace MiniGram.Api
{
    public partial class Program { /* for testing */ }
}
