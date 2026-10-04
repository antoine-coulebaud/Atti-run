using Microsoft.EntityFrameworkCore;

namespace AttiRun.WebApp.Models;

public class AttiRunDb(DbContextOptions<AttiRunDb> options) : DbContext(options)
{
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Sortie> Sorties => Set<Sortie>();

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sortie>()
            .HasMany(s => s.Participants)
            .WithMany();

        modelBuilder.Entity<Sortie>()
            .Navigation(s => s.Participants)
            .AutoInclude();
    }
}