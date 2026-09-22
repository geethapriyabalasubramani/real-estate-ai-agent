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

            entity.HasIndex(p => p.City)
                .HasDatabaseName("IX_Properties_City");

            entity.HasIndex(p => p.Bedrooms)
                .HasDatabaseName("IX_Properties_Bedrooms");

            entity.HasIndex(p => p.HasGarage)
                .HasDatabaseName("IX_Properties_HasGarage");

            entity.HasIndex(p => p.Price)
                .HasDatabaseName("IX_Properties_Price");

            entity.HasIndex(p => new { p.City, p.Bedrooms, p.Price })
                .HasDatabaseName("IX_Properties_City_Bedrooms_Price");
        });
    }
}