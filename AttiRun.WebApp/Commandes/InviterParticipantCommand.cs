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

        sortie.InviterParticipant(invite, inviteur);

        await sortiesRepository.PersisterSortieAsync(sortie, cancellationToken);

        return sortie;
    }
}