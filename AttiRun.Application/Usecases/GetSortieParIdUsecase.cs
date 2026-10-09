using AttiRun.Application.Contracts.Repositories;
using AttiRun.Application.Contracts.Usecases;
using AttiRun.Domain.Entities;

namespace AttiRun.Application.Usecases;

public class GetSortieParIdUsecase(Guid id, ISortiesRepository sortiesRepository) : IUsecase<Sortie?>
{
    public Task<Sortie?> HandleAsync(CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortieParIdAsync(id, cancellationToken);
    }
}
