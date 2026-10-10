using AttiRun.Application.Commandes;
using AttiRun.Application.Contracts.Repositories;
using AttiRun.Domain.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AttiRun.Tests.Application.Commandes;

public class InscrireParticipantCommandTests
{
    private const string NomParticipant = "Alice";

    private readonly IParticipantsRepository _participantsRepository = Substitute.For<IParticipantsRepository>();
    private readonly ISortiesRepository _sortiesRepository = Substitute.For<ISortiesRepository>();
    private readonly InscrireParticipantCommandHandler _handler;

    private readonly Sortie _sortie = new()
    {
        Nom = "Sortie test",
        Localisation = "Paris",
        Date = DateTime.Now.AddDays(7),
        NombreParticipantsMax = 10,
        Statut = StatutSortie.Ouverte,
    };

    private readonly Participant _participant = new() { Nom = NomParticipant };

    public InscrireParticipantCommandTests()
    {
        _handler = new InscrireParticipantCommandHandler(_participantsRepository, _sortiesRepository);

        _sortiesRepository.GetSortieParIdAsync(_sortie.Id, Arg.Any<CancellationToken>()).Returns(_sortie);
        _participantsRepository.GetParticipantParNom(NomParticipant, Arg.Any<CancellationToken>()).Returns(_participant);
    }

    private InscrireParticipantCommand CreerCommande() => new(_sortie.Id, NomParticipant);

    [Fact(DisplayName = "Inscrire un participant à une sortie inexistante lève une exception")]
    public async Task Handle_SortieInexistante_LeveInvalidOperationException()
    {
        _sortiesRepository.GetSortieParIdAsync(_sortie.Id, Arg.Any<CancellationToken>()).Returns((Sortie?)null);

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("La sortie n'existe pas.");
        await _sortiesRepository.DidNotReceiveWithAnyArgs().PersisterSortieAsync(default!, default);
    }

    [Fact(DisplayName = "Inscrire un participant inexistant lève une exception")]
    public async Task Handle_ParticipantInexistant_LeveInvalidOperationException()
    {
        _participantsRepository.GetParticipantParNom(NomParticipant, Arg.Any<CancellationToken>()).Returns((Participant?)null);

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Participant non trouvé.");
        _sortie.Participants.Should().BeEmpty();
        await _sortiesRepository.DidNotReceiveWithAnyArgs().PersisterSortieAsync(default!, default);
    }

    [Fact(DisplayName = "Inscrire un participant à une sortie fermée lève une exception sans persister la sortie")]
    public async Task Handle_SortieFermee_LeveInvalidOperationExceptionSansPersister()
    {
        _sortie.Statut = StatutSortie.Annulee;

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("La sortie n'est pas ouverte aux inscriptions.");
        _sortie.Participants.Should().BeEmpty();
        await _sortiesRepository.DidNotReceiveWithAnyArgs().PersisterSortieAsync(default!, default);
    }

    [Fact(DisplayName = "Inscrire un participant propage l'exception quand la persistance échoue")]
    public async Task Handle_PersistanceEchoue_PropageLException()
    {
        var erreurPersistance = new InvalidOperationException("Erreur de persistance.");
        _sortiesRepository.PersisterSortieAsync(_sortie, Arg.Any<CancellationToken>()).ThrowsAsync(erreurPersistance);

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        (await handle.Should().ThrowAsync<InvalidOperationException>())
            .Which.Should().BeSameAs(erreurPersistance);
    }

    [Fact(DisplayName = "Inscrire un participant à une sortie ouverte l'ajoute aux participants et persiste la sortie")]
    public async Task Handle_CommandeValide_InscritLeParticipantEtPersisteLaSortie()
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        var resultat = await _handler.Handle(CreerCommande(), cancellationTokenSource.Token);

        resultat.Should().BeSameAs(_sortie);
        resultat.Participants.Should().ContainSingle()
            .Which.Should().BeSameAs(_participant);
        await _sortiesRepository.Received(1).PersisterSortieAsync(_sortie, cancellationTokenSource.Token);
    }
}
