using AttiRun.Domain.Entities;

namespace AttiRun.Application.Contracts.Repositories;

public interface ISortiesRepository
{
    Task<Sortie?> GetSortieParIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Sortie>> GetSortiesAsync(CancellationToken cancellationToken);
    Task PersisterSortieAsync(Sortie sortie, CancellationToken cancellationToken);
}
