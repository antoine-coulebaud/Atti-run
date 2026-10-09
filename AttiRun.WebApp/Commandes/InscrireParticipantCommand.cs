using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;
using MediatR;

namespace AttiRun.WebApp.Commandes;

public record InscrireParticipantCommand(Guid SortieId, string NomParticipant) : IRequest<Sortie>;

public class InscrireParticipantCommandHandler(IParticipantsRepository participantsRepository, ISortiesRepository sortiesRepository)
    : IRequestHandler<InscrireParticipantCommand, Sortie>
{
    public async Task<Sortie> Handle(InscrireParticipantCommand command, CancellationToken cancellationToken)
    {
        var sortie = await sortiesRepository.GetSortieParIdAsync(command.SortieId, cancellationToken)
            ?? throw new InvalidOperationException("La sortie n'existe pas.");

        var participant = await participantsRepository.GetParticipantParNom(command.NomParticipant, cancellationToken)
            ?? throw new InvalidOperationException("Participant non trouvé.");

        var sortieEstOuverteAuxInscriptions = sortie.Statut == StatutSortie.Ouverte &&
            sortie.Date > DateTime.Now &&
            sortie.Participants.Count < sortie.NombreParticipantsMax;

        if (!sortieEstOuverteAuxInscriptions)
        {
            throw new InvalidOperationException("La sortie n'est pas ouverte aux inscriptions.");
        }

        if (sortie.Participants.Any(p => p.Id == participant.Id))
        {
            throw new InvalidOperationException("Le participant est déjà inscrit à cette sortie.");
        }

        sortie.Participants.Add(participant);

        await sortiesRepository.PersisterSortieAsync(sortie, cancellationToken);

        return sortie;
    }
}