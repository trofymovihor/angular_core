using System.Text;
using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
var movieApiKey = builder.Configuration["Movies:ServiceApiKey"];

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x =>
{
    x.AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "an error occurred during migration");
}

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
var test = config["AppSecret"];
Console.WriteLine(config["AppSecret"]);

app.Run();
