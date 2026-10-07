using AttiRun.WebApp.Models;
using AttiRun.WebApp.Repositories;

namespace AttiRun.WebApp.Services;

public class SortiesService(AttiRunDb dbContext)
{
    private readonly SortiesRepository sortiesRepository = new(dbContext);
    private readonly ParticipantsRepository participantsRepository = new(dbContext);

    public async Task<IEnumerable<Sortie>> GetSorties(CancellationToken cancellationToken)
    {
        return await sortiesRepository.GetSorties(cancellationToken);
    }

    public async Task<Sortie?> GetSortieParId(Guid id, CancellationToken cancellationToken)
    {
        return await sortiesRepository.GetSortieParId(id, cancellationToken);
    }

    public async Task<Sortie> InscrireParticipant(
        Sortie sortie,
        string nomParticipant,
        CancellationToken cancellationToken)
    {
        var participant = await participantsRepository.GetParticipantParNom(nomParticipant, cancellationToken)
            ?? throw new InvalidOperationException("Participant non trouvé.");

        if (!SortieEstOuverteAuxInscriptions(sortie))
        {
            throw new InvalidOperationException("La sortie n'est pas ouverte aux inscriptions.");
        }

        if (sortie.Participants.Any(p => p.Id == participant.Id))
        {
            throw new InvalidOperationException("Le participant est déjà inscrit à cette sortie.");
        }

        sortie.Participants.Add(participant);
        
        await sortiesRepository.PersisterSortie(sortie, cancellationToken);

        return sortie;
    }

    public async Task<Sortie> InviterParticipant(
        Sortie sortie,
        string nomParticipant,
        string nomInvite,
        CancellationToken cancellationToken)
    {
        var participant = await participantsRepository.GetParticipantParNom(nomParticipant, cancellationToken)
            ?? throw new InvalidOperationException("Participant non trouvé.");

        var invite = await participantsRepository.GetParticipantParNom(nomInvite, cancellationToken)
            ?? throw new InvalidOperationException("Participant à inviter non trouvé.");

        if (!SortieEstOuverteAuxInscriptions(sortie))
        {
            throw new InvalidOperationException("La sortie n'est pas ouverte aux inscriptions.");
        }

        if (sortie.Participants.Any(p => p.Id == invite.Id))
        {
            throw new InvalidOperationException("Le participant est déjà inscrit à cette sortie.");
        }

        if (sortie.Invitations.Any(i => i.Invite.Id == invite.Id))
        {
            throw new InvalidOperationException("Le participant a déjà été invité à cette sortie.");
        }

        var invitation = new Invitation
        {
            Invite = invite,
            Inviteur = participant,
        };

        sortie.Invitations.Add(invitation);

        await sortiesRepository.PersisterSortie(sortie, cancellationToken);

        return sortie;
    }

    private static bool SortieEstOuverteAuxInscriptions(Sortie sortie)
    {
        return sortie.Statut == StatutSortie.Ouverte &&
            sortie.Date > DateTime.Now &&
            sortie.Participants.Count < sortie.NombreParticipantsMax;
    }
}
