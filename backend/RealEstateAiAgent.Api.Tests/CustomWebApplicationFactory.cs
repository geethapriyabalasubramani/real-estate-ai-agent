using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstateAiAgent.Api.Data;

namespace RealEstateAiAgent.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly object SeedLock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "RealEstateAiAgent.Tests",
                ["Jwt:Audience"] = "RealEstateAiAgent.Tests",
                ["Jwt:Key"] = "test-secret-key-at-least-32-characters-long!!",
                ["Jwt:ExpireMinutes"] = "60",
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        lock (SeedLock)
        {
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            if (!db.Properties.Any())
            {
                db.Properties.AddRange(PropertySeedData.GetProperties());
                db.SaveChanges();
            }
        }

        return host;
    }
}
