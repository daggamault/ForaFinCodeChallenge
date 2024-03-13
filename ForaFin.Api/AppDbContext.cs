using ForaFin.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForaFin.Api;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<CompanyEntity> CompanyInfos { get; init; } = null!;
}