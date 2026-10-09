using AttiRun.Application.Contracts.Repositories;
using AttiRun.Domain.Entities;
using MediatR;

namespace AttiRun.Application.Commandes;

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

        sortie.Participer(participant);

        await sortiesRepository.PersisterSortieAsync(sortie, cancellationToken);

        return sortie;
    }
}