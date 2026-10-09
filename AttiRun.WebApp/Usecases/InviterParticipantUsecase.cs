using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Contracts.Usecases;
using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Usecases;

public class InviterParticipantUsecase(
    Guid sortieId,
    string nomInviteur,
    string nomInvite,
    ISortiesRepository sortiesRepository,
    IParticipantsRepository participantsRepository) : IUsecase<Sortie>
{
    public async Task<Sortie> HandleAsync(CancellationToken cancellationToken)
    {
        var sortie = await sortiesRepository.GetSortieParIdAsync(sortieId, cancellationToken)
            ?? throw new InvalidOperationException("La sortie n'existe pas.");

        var inviteur = await participantsRepository.GetParticipantParNom(nomInviteur, cancellationToken)
            ?? throw new InvalidOperationException("Inviteur non trouvé.");

        var invite = await participantsRepository.GetParticipantParNom(nomInvite, cancellationToken)
            ?? throw new InvalidOperationException("Invite non trouvé.");

        sortie.InviterParticipant(invite, inviteur);

        await sortiesRepository.PersisterSortieAsync(sortie, cancellationToken);

        return sortie;
    }
}
