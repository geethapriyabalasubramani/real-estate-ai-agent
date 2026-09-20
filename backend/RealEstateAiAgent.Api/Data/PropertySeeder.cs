using Microsoft.EntityFrameworkCore;
using RealEstateAiAgent.Api.Models;

namespace RealEstateAiAgent.Api.Data;

public static class PropertySeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Properties.AnyAsync(cancellationToken))
        {
            return;
        }

        var properties = PropertySeedData.GetProperties();
        await db.Properties.AddRangeAsync(properties, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}