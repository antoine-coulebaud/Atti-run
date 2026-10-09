using AttiRun.Application.Contracts.Repositories;
using AttiRun.Application.Contracts.Usecases;
using AttiRun.Domain.Entities;

namespace AttiRun.Application.Usecases;

public class GetSortiesUsecase(ISortiesRepository sortiesRepository) : IUsecase<IEnumerable<Sortie>>
{
    public Task<IEnumerable<Sortie>> HandleAsync(CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortiesAsync(cancellationToken);
    }
}
