using Microsoft.EntityFrameworkCore;

namespace RealEstateAiAgent.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}