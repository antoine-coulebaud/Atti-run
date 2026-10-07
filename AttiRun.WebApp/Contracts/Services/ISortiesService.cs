using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Contracts.Services;

public interface ISortiesService
{
    Task<Sortie?> GetSortieParId(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Sortie>> GetSorties(CancellationToken cancellationToken);
    Task<Sortie> InscrireParticipant(Sortie sortie, string nomParticipant, CancellationToken cancellationToken);
    Task<Sortie> InviterParticipant(Sortie sortie, string nomParticipant, string nomInvite, CancellationToken cancellationToken);
}