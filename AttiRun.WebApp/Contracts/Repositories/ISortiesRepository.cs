using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Contracts.Repositories;

public interface ISortiesRepository
{
    Task<Sortie?> GetSortieParId(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Sortie>> GetSorties(CancellationToken cancellationToken);
    Task PersisterSortie(Sortie sortie, CancellationToken cancellationToken);
}
