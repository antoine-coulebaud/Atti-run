using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;
using MediatR;

namespace AttiRun.WebApp.Commandes;

public record InviterParticipantCommand(Guid SortieId, string NomParticipant, string NomInvite) : IRequest<Sortie>;

public class InviterParticipantCommandHandler(IParticipantsRepository participantsRepository, ISortiesRepository sortiesRepository)
    : IRequestHandler<InviterParticipantCommand, Sortie>
{
    public async Task<Sortie> Handle(InviterParticipantCommand command, CancellationToken cancellationToken)
    {
        var sortie = await sortiesRepository.GetSortieParIdAsync(command.SortieId, cancellationToken)
            ?? throw new InvalidOperationException("La sortie n'existe pas.");

        var inviteur = await participantsRepository.GetParticipantParNom(command.NomParticipant, cancellationToken)
            ?? throw new InvalidOperationException("Inviteur non trouvé.");

        var invite = await participantsRepository.GetParticipantParNom(command.NomInvite, cancellationToken)
            ?? throw new InvalidOperationException("Invite non trouvé.");

        var sortieEstOuverteAuxInscriptions = sortie.Statut == StatutSortie.Ouverte &&
            sortie.Date > DateTime.Now &&
            sortie.Participants.Count < sortie.NombreParticipantsMax;

        if (!sortieEstOuverteAuxInscriptions)
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
            Inviteur = inviteur,
        };

        sortie.Invitations.Add(invitation);

        await sortiesRepository.PersisterSortieAsync(sortie, cancellationToken);

        return sortie;
    }
}