using AttiRun.WebApp.Models;
using AttiRun.WebApp.Repositories.InMemory;
using AttiRun.WebApp.Services;

namespace AttiRun.Tests.Services;

public class SortiesServiceTests
{
    [Fact(DisplayName = "InscrireParticipant doit lever une exception si le participant n'existe pas")]
    public async Task InscrireParticipant_DoitLeverUneException_SiLeParticipantNExsistePas()
    {
        // Arrange
        var sortiesRepository = InMemorySortiesRepository.CreerRepositoryVide();
        var participantsRepository = InMemoryParticipantsRepository.CreerRepositoryVide();

        var sortie = new Sortie
        {
            Id = Guid.NewGuid(),
            Nom = "SortieTest",
            Date = DateTime.Now,
            Participants = [],
        };

        var sut = new SortiesService(sortiesRepository, participantsRepository);


        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.InscrireParticipant(sortie, "Participant", CancellationToken.None));
    }

    [Fact(DisplayName = "InscrireParticipant doit retourner la sortie avec le participant inscrit si l'inscription réussit")]
    public async Task InscrireParticipant_DoitRetournerLaSortieAvecLeParticipantInscrit_SiLInscriptionReussit()
    {
        // Arrange
        var sortie = new Sortie
        {
            Id = Guid.NewGuid(),
            Nom = "SortieTest",
            Date = DateTime.Now.AddDays(1),
            NombreParticipantsMax = 10,
            Statut = StatutSortie.Ouverte,
            Participants = [],
        };

        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            Nom = "Participant",
        };

        var sortiesRepository = InMemorySortiesRepository.CreerRepositoryVide();
        sortiesRepository.AjouterSortie(sortie);

        var participantsRepository = InMemoryParticipantsRepository.CreerRepositoryVide();
        participantsRepository.AjouterParticipant(participant);

        var sut = new SortiesService(sortiesRepository, participantsRepository);

        // Act
        var result = await sut.InscrireParticipant(sortie, participant.Nom, CancellationToken.None);

        // Assert
        Assert.Contains(result.Participants, p => p.Id == participant.Id);
    }
}
