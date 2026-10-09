using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Contracts.Usecases;
using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Usecases;

public class GetSortieParIdUsecase(Guid id, ISortiesRepository sortiesRepository) : IUsecase<Sortie?>
{
    public Task<Sortie?> HandleAsync(CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortieParIdAsync(id, cancellationToken);
    }
}
