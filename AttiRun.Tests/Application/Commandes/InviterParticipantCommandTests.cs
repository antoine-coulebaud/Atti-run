using AttiRun.Application.Commandes;
using AttiRun.Application.Contracts.Repositories;
using AttiRun.Domain.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace AttiRun.Tests.Application.Commandes;

public class InviterParticipantCommandTests
{
    private const string NomInviteur = "Bob";
    private const string NomInvite = "Alice";

    private readonly IParticipantsRepository _participantsRepository = Substitute.For<IParticipantsRepository>();
    private readonly ISortiesRepository _sortiesRepository = Substitute.For<ISortiesRepository>();
    private readonly InviterParticipantCommandHandler _handler;

    private readonly Sortie _sortie = new()
    {
        Nom = "Sortie test",
        Localisation = "Paris",
        Date = DateTime.Now.AddDays(7),
        NombreParticipantsMax = 10,
        Statut = StatutSortie.Ouverte,
    };

    private readonly Participant _inviteur = new() { Nom = NomInviteur };
    private readonly Participant _invite = new() { Nom = NomInvite };

    public InviterParticipantCommandTests()
    {
        _handler = new InviterParticipantCommandHandler(_participantsRepository, _sortiesRepository);

        _sortiesRepository.GetSortieParIdAsync(_sortie.Id, Arg.Any<CancellationToken>()).Returns(_sortie);
        _participantsRepository.GetParticipantParNom(NomInviteur, Arg.Any<CancellationToken>()).Returns(_inviteur);
        _participantsRepository.GetParticipantParNom(NomInvite, Arg.Any<CancellationToken>()).Returns(_invite);
    }

    private InviterParticipantCommand CreerCommande() => new(_sortie.Id, NomInviteur, NomInvite);

    [Fact(DisplayName = "Inviter un participant à une sortie inexistante lève une exception")]
    public async Task Handle_SortieInexistante_LeveInvalidOperationException()
    {
        _sortiesRepository.GetSortieParIdAsync(_sortie.Id, Arg.Any<CancellationToken>()).Returns((Sortie?)null);

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("La sortie n'existe pas.");
        await _sortiesRepository.DidNotReceiveWithAnyArgs().PersisterSortieAsync(default!, default);
    }

    [Fact(DisplayName = "Inviter un participant de la part d'un inviteur inexistant lève une exception")]
    public async Task Handle_InviteurInexistant_LeveInvalidOperationException()
    {
        _participantsRepository.GetParticipantParNom(NomInviteur, Arg.Any<CancellationToken>()).Returns((Participant?)null);

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Inviteur non trouvé.");
        _sortie.Invitations.Should().BeEmpty();
        await _sortiesRepository.DidNotReceiveWithAnyArgs().PersisterSortieAsync(default!, default);
    }

    [Fact(DisplayName = "Inviter un participant inexistant lève une exception")]
    public async Task Handle_InviteInexistant_LeveInvalidOperationException()
    {
        _participantsRepository.GetParticipantParNom(NomInvite, Arg.Any<CancellationToken>()).Returns((Participant?)null);

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invite non trouvé.");
        _sortie.Invitations.Should().BeEmpty();
        await _sortiesRepository.DidNotReceiveWithAnyArgs().PersisterSortieAsync(default!, default);
    }

    [Fact(DisplayName = "Inviter un participant propage l'exception quand la persistance échoue")]
    public async Task Handle_PersistanceEchoue_PropageLException()
    {
        var erreurPersistance = new InvalidOperationException("Erreur de persistance.");
        _sortiesRepository.PersisterSortieAsync(_sortie, Arg.Any<CancellationToken>()).ThrowsAsync(erreurPersistance);

        var handle = () => _handler.Handle(CreerCommande(), CancellationToken.None);

        (await handle.Should().ThrowAsync<InvalidOperationException>())
            .Which.Should().BeSameAs(erreurPersistance);
    }

    [Fact(DisplayName = "Inviter un participant à une sortie ouverte crée l'invitation et persiste la sortie")]
    public async Task Handle_CommandeValide_InviteLeParticipantEtPersisteLaSortie()
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        var resultat = await _handler.Handle(CreerCommande(), cancellationTokenSource.Token);

        resultat.Should().BeSameAs(_sortie);
        var invitation = resultat.Invitations.Should().ContainSingle().Subject;
        invitation.Invite.Should().BeSameAs(_invite);
        invitation.Inviteur.Should().BeSameAs(_inviteur);
        await _sortiesRepository.Received(1).PersisterSortieAsync(_sortie, cancellationTokenSource.Token);
    }
}
