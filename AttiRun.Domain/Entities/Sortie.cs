namespace AttiRun.Domain.Entities;

public class Sortie
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Nom { get; set; } = string.Empty;
    public string Localisation { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int NombreParticipantsMax { get; set; }
    public StatutSortie Statut { get; set; }

    private readonly List<Participant> _participants = [];
    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

    private readonly List<Invitation> _invitations = [];
    public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();

    private bool EstOuverteAuxInscriptions => Statut == StatutSortie.Ouverte
        && Date > DateTime.Now
        && Participants.Count < NombreParticipantsMax;

    public void Participer(Participant participant)
    {
        if (!EstOuverteAuxInscriptions)
        {
            throw new InvalidOperationException("La sortie n'est pas ouverte aux inscriptions.");
        }

        if (Participants.Any(p => p.Id == participant.Id))
        {
            throw new InvalidOperationException($"Le participant {participant.Nom} est déjà inscrit à cette sortie.");
        }

        _participants.Add(participant);
    }

    public void InviterParticipant(Participant invite, Participant inviteur)
    {
        if (!EstOuverteAuxInscriptions)
        {
            throw new InvalidOperationException("La sortie n'est pas ouverte aux inscriptions.");
        }

        if (Participants.Any(p => p.Id == invite.Id))
        {
            throw new InvalidOperationException($"Le participant {invite.Nom} est déjà inscrit à cette sortie.");
        }

        if (Invitations.Any(i => i.Invite.Id == invite.Id))
        {
            throw new InvalidOperationException($"Le participant {invite.Nom} est déjà invité à cette sortie.");
        }

        var invitation = new Invitation
        {
            Invite = invite,
            Inviteur = inviteur,
        };

        _invitations.Add(invitation);
    }
}