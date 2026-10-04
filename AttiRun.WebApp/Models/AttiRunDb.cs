using Microsoft.EntityFrameworkCore;

namespace AttiRun.WebApp.Models;

public class AttiRunDb(DbContextOptions<AttiRunDb> options) : DbContext(options)
{
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Sortie> Sorties => Set<Sortie>();
    public DbSet<Invitation> Invitations => Set<Invitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participant>()
            .Property(p => p.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<Sortie>()
            .Property(s => s.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<Sortie>()
            .HasMany(s => s.Participants)
            .WithMany();

        modelBuilder.Entity<Sortie>()
            .Navigation(s => s.Participants)
            .AutoInclude();

        modelBuilder.Entity<Invitation>()
            .Property(i => i.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<Invitation>()
            .HasOne(typeof(Sortie))
            .WithMany("Invitations")
            .OnDelete(DeleteBehavior.Cascade);
    }
}