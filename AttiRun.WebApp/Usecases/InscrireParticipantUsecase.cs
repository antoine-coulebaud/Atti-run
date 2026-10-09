using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Contracts.Usecases;
using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Usecases;

public class InscrireParticipantUsecase(
    Guid sortieId,
    string nomParticipant,
    ISortiesRepository sortiesRepository,
    IParticipantsRepository participantsRepository) : IUsecase<Sortie>
{
    public async Task<Sortie> HandleAsync(CancellationToken cancellationToken)
    {
        var sortie = await sortiesRepository.GetSortieParIdAsync(sortieId, cancellationToken)
            ?? throw new InvalidOperationException("La sortie n'existe pas.");

        var participant = await participantsRepository.GetParticipantParNom(nomParticipant, cancellationToken)
            ?? throw new InvalidOperationException("Participant non trouvé.");

        sortie.Participer(participant);

        await sortiesRepository.PersisterSortieAsync(sortie, cancellationToken);

        return sortie;
    }
}
