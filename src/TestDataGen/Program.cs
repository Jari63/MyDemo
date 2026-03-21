using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyDemo.Infrastructure.Data;
using MyDemo.Infrastructure.Identity;

var drop = args.Contains("--drop");

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MyDemoDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'MyDemoDb' not found or is empty.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDataProtection();
builder.Services.AddScoped<ApplicationDbContextInitialiser>();
builder.Services.AddSingleton(TimeProvider.System);

var host = builder.Build();

using var scope = host.Services.CreateScope();
var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

if (drop)
{
    logger.LogInformation("Dropping database...");
    await initialiser.DropAsync();
}

logger.LogInformation("Applying migrations...");
await initialiser.MigrateAsync();

logger.LogInformation("Seeding database...");
await initialiser.SeedAsync();

logger.LogInformation("Done.");
