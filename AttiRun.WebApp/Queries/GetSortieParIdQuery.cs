using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;
using MediatR;

namespace AttiRun.WebApp.Queries;

public record GetSortieParIdQuery(Guid SortieId) : IRequest<Sortie?>;

public class GetSortieParIdQueryHandler(ISortiesRepository sortiesRepository) : IRequestHandler<GetSortieParIdQuery, Sortie?>
{
    public Task<Sortie?> Handle(GetSortieParIdQuery query, CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortieParIdAsync(query.SortieId, cancellationToken);
    }
}