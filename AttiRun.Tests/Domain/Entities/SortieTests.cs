using AttiRun.Domain.Entities;

namespace AttiRun.Tests.Domain.Entities;

public class SortieTests
{
    private static Sortie CreerSortieOuverte(int nombreParticipantsMax = 10) => new()
    {
        Nom = "Sortie test",
        Localisation = "Paris",
        Date = DateTime.Now.AddDays(7),
        NombreParticipantsMax = nombreParticipantsMax,
        Statut = StatutSortie.Ouverte,
    };

    [Fact(DisplayName = "Participer à une sortie annulée lève une exception")]
    public void Participer_SortieAnnulee_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte();
        sortie.Statut = StatutSortie.Annulee;
        var participant = new Participant { Nom = "Alice" };

        var participer = () => sortie.Participer(participant);

        participer.Should().Throw<InvalidOperationException>()
            .WithMessage("La sortie n'est pas ouverte aux inscriptions.");
        sortie.Participants.Should().BeEmpty();
    }

    [Fact(DisplayName = "Participer à une sortie passée lève une exception")]
    public void Participer_SortiePassee_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte();
        sortie.Date = DateTime.Now.AddDays(-1);
        var participant = new Participant { Nom = "Alice" };

        var participer = () => sortie.Participer(participant);

        participer.Should().Throw<InvalidOperationException>()
            .WithMessage("La sortie n'est pas ouverte aux inscriptions.");
        sortie.Participants.Should().BeEmpty();
    }

    [Fact(DisplayName = "Participer à une sortie complète lève une exception")]
    public void Participer_SortieComplete_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte(nombreParticipantsMax: 1);
        var premierParticipant = new Participant { Nom = "Alice" };
        sortie.Participer(premierParticipant);
        var participant = new Participant { Nom = "Bob" };

        var participer = () => sortie.Participer(participant);

        participer.Should().Throw<InvalidOperationException>()
            .WithMessage("La sortie n'est pas ouverte aux inscriptions.");
        sortie.Participants.Should().ContainSingle()
            .Which.Should().BeSameAs(premierParticipant);
    }

    [Fact(DisplayName = "Participer à une sortie à laquelle on est déjà inscrit lève une exception")]
    public void Participer_ParticipantDejaInscrit_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte();
        var participant = new Participant { Nom = "Alice" };
        sortie.Participer(participant);

        var participer = () => sortie.Participer(participant);

        participer.Should().Throw<InvalidOperationException>()
            .WithMessage("Le participant Alice est déjà inscrit à cette sortie.");
        sortie.Participants.Should().ContainSingle();
    }

    [Fact(DisplayName = "Participer à une sortie ouverte inscrit le participant")]
    public void Participer_SortieOuverte_AjouteLeParticipant()
    {
        var sortie = CreerSortieOuverte();
        var participant = new Participant { Nom = "Alice" };

        sortie.Participer(participant);

        sortie.Participants.Should().ContainSingle()
            .Which.Should().BeSameAs(participant);
    }

    [Fact(DisplayName = "Inviter un participant à une sortie annulée lève une exception")]
    public void InviterParticipant_SortieAnnulee_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte();
        sortie.Statut = StatutSortie.Annulee;
        var invite = new Participant { Nom = "Alice" };
        var inviteur = new Participant { Nom = "Bob" };

        var inviter = () => sortie.InviterParticipant(invite, inviteur);

        inviter.Should().Throw<InvalidOperationException>()
            .WithMessage("La sortie n'est pas ouverte aux inscriptions.");
        sortie.Invitations.Should().BeEmpty();
    }

    [Fact(DisplayName = "Inviter un participant à une sortie passée lève une exception")]
    public void InviterParticipant_SortiePassee_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte();
        sortie.Date = DateTime.Now.AddDays(-1);
        var invite = new Participant { Nom = "Alice" };
        var inviteur = new Participant { Nom = "Bob" };

        var inviter = () => sortie.InviterParticipant(invite, inviteur);

        inviter.Should().Throw<InvalidOperationException>()
            .WithMessage("La sortie n'est pas ouverte aux inscriptions.");
        sortie.Invitations.Should().BeEmpty();
    }

    [Fact(DisplayName = "Inviter un participant à une sortie complète lève une exception")]
    public void InviterParticipant_SortieComplete_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte(nombreParticipantsMax: 1);
        var inviteur = new Participant { Nom = "Bob" };
        sortie.Participer(inviteur);
        var invite = new Participant { Nom = "Alice" };

        var inviter = () => sortie.InviterParticipant(invite, inviteur);

        inviter.Should().Throw<InvalidOperationException>()
            .WithMessage("La sortie n'est pas ouverte aux inscriptions.");
        sortie.Invitations.Should().BeEmpty();
    }

    [Fact(DisplayName = "Inviter un participant déjà inscrit à la sortie lève une exception")]
    public void InviterParticipant_ParticipantDejaInscrit_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte();
        var invite = new Participant { Nom = "Alice" };
        var inviteur = new Participant { Nom = "Bob" };
        sortie.Participer(invite);

        var inviter = () => sortie.InviterParticipant(invite, inviteur);

        inviter.Should().Throw<InvalidOperationException>()
            .WithMessage("Le participant Alice est déjà inscrit à cette sortie.");
        sortie.Invitations.Should().BeEmpty();
    }

    [Fact(DisplayName = "Inviter un participant déjà invité à la sortie lève une exception")]
    public void InviterParticipant_ParticipantDejaInvite_LeveInvalidOperationException()
    {
        var sortie = CreerSortieOuverte();
        var invite = new Participant { Nom = "Alice" };
        var inviteur = new Participant { Nom = "Bob" };
        sortie.InviterParticipant(invite, inviteur);

        var inviter = () => sortie.InviterParticipant(invite, inviteur);

        inviter.Should().Throw<InvalidOperationException>()
            .WithMessage("Le participant Alice est déjà invité à cette sortie.");
        sortie.Invitations.Should().ContainSingle();
    }

    [Fact(DisplayName = "Inviter un participant à une sortie ouverte crée une invitation en attente")]
    public void InviterParticipant_SortieOuverte_AjouteUneInvitationEnAttente()
    {
        var sortie = CreerSortieOuverte();
        var invite = new Participant { Nom = "Alice" };
        var inviteur = new Participant { Nom = "Bob" };

        sortie.InviterParticipant(invite, inviteur);

        var invitation = sortie.Invitations.Should().ContainSingle().Subject;
        invitation.Invite.Should().BeSameAs(invite);
        invitation.Inviteur.Should().BeSameAs(inviteur);
        invitation.Statut.Should().Be(StatutInvitation.EnAttente);
        sortie.Participants.Should().BeEmpty();
    }
}
