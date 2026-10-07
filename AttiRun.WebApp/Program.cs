using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Contracts.Services;
using AttiRun.WebApp.Models;
using AttiRun.WebApp.Repositories;
using AttiRun.WebApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AttiRunDb>(options =>
    options.UseInMemoryDatabase("atti-run-db"));

builder.Services.AddScoped<ISortiesRepository, SortiesRepository>();
builder.Services.AddScoped<IParticipantsRepository, ParticipantsRepository>();
builder.Services.AddScoped<ISortiesService, SortiesService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

SeedInMemoryDb();

await app.RunAsync().ConfigureAwait(false);
return;

void SeedInMemoryDb()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AttiRunDb>();

    db.Database.EnsureCreated();

    if (!db.Sorties.Any())
    {
        var participants = new List<Participant>
        {
            new() { Nom = "Alice" },
            new() { Nom = "Bob" },
            new() { Nom = "Charlie" },
        };

        db.Participants.AddRange(participants);

        db.Sorties.AddRange(
            new Sortie {
                Nom = "5km chill",
                Date = new DateTime(2027, 06, 20),
                Localisation = "Quai rive droite",
                NombreParticipantsMax = 10,
                Statut = StatutSortie.Ouverte,
                Participants = [participants[1]],
            },
            new Sortie {
                Nom = "Prépa 10km de Rouen",
                Date = new DateTime(2027, 06, 15),
                NombreParticipantsMax = 5,
                Localisation = "Quai rive droite",
                Statut = StatutSortie.Ouverte,
                Participants = [participants[2]],
            }
        );
        db.SaveChanges();
    }
}