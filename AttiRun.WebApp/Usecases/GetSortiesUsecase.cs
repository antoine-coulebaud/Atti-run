using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Contracts.Usecases;
using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Usecases;

public class GetSortiesUsecase(ISortiesRepository sortiesRepository) : IUsecase<IEnumerable<Sortie>>
{
    public Task<IEnumerable<Sortie>> HandleAsync(CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortiesAsync(cancellationToken);
    }
}
