using Microsoft.EntityFrameworkCore;
using RealEstateAiAgent.Api.Models;

namespace RealEstateAiAgent.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Property> Properties => Set<Property>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("Properties");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.AddressLine1).HasMaxLength(200).IsRequired();
            entity.Property(p => p.City).HasMaxLength(100).IsRequired();
            entity.Property(p => p.State).HasMaxLength(2).IsRequired();
            entity.Property(p => p.PostalCode).HasMaxLength(10).IsRequired();

            entity.Property(p => p.Bedrooms).IsRequired();
            entity.Property(p => p.Bathrooms).HasPrecision(3, 1);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.SquareFeet).IsRequired();

            entity.Property(p => p.Description).HasMaxLength(4000);

            entity.Property(p => p.CreatedAtUtc).IsRequired();
        });
    }
}