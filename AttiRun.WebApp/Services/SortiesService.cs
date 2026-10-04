using AttiRun.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AttiRun.WebApp.Services;

public class SortiesService(AttiRunDb dbContext)
{
    public async Task<Sortie?> GetSortieParId(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Sorties
            .Include(s => s.Invitations)
                .ThenInclude(i => i.Invite)
            .Include(s => s.Invitations)
                .ThenInclude(i => i.Inviteur)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sortie> InscrireParticipant(
        Sortie sortie,
        string nomParticipant,
        CancellationToken cancellationToken)
    {
        var participant = await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nomParticipant, cancellationToken)
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
        await dbContext.SaveChangesAsync(cancellationToken);

        return sortie;
    }

    public async Task<Sortie> InviterParticipant(
        Sortie sortie,
        string nomParticipant,
        string nomInvite,
        CancellationToken cancellationToken)
    {
        var participant = await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nomParticipant, cancellationToken)
            ?? throw new InvalidOperationException("Participant non trouvé.");

        var invite = await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nomInvite, cancellationToken)
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

        dbContext.Invitations.Add(invitation);
        dbContext.Update(sortie);
        await dbContext.SaveChangesAsync(cancellationToken);

        return sortie;
    }

    private static bool SortieEstOuverteAuxInscriptions(Sortie sortie)
    {
        return sortie.Statut == StatutSortie.Ouverte &&
            sortie.Date > DateTime.Now &&
            sortie.Participants.Count < sortie.NombreParticipantsMax;
    }
}
