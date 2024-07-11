using Microsoft.EntityFrameworkCore;
using AisStreamService.Models;


namespace AisStreamService.Data;

public class AisDbContext : DbContext
{
    public AisDbContext(DbContextOptions<AisDbContext> options) : base(options) { }

    public DbSet<Vessel> Vessels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vessel>().ToTable("AIS_Vessels");
    }
}