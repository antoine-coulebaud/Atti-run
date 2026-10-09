using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Contracts.Repositories;

public interface ISortiesRepository
{
    Task<Sortie?> GetSortieParIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Sortie>> GetSortiesAsync(CancellationToken cancellationToken);
    Task PersisterSortieAsync(Sortie sortie, CancellationToken cancellationToken);
}
