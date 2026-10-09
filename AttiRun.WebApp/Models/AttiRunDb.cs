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
            .HasMany<Participant>(nameof(Sortie.Participants))
            .WithMany();

        modelBuilder.Entity<Sortie>()
            .Navigation(nameof(Sortie.Participants))
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude();

        modelBuilder.Entity<Invitation>()
            .Property(i => i.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Sortie>()
            .HasMany<Invitation>(nameof(Sortie.Invitations))
            .WithOne();

        modelBuilder.Entity<Sortie>()
            .Navigation(nameof(Sortie.Invitations))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}