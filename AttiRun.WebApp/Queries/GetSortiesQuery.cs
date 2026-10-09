using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;
using MediatR;

namespace AttiRun.WebApp.Queries;

public record GetSortiesQuery : IRequest<IEnumerable<Sortie>>;

public class GetSortiesQueryHandler(ISortiesRepository sortiesRepository) : IRequestHandler<GetSortiesQuery, IEnumerable<Sortie>>
{
    public Task<IEnumerable<Sortie>> Handle(GetSortiesQuery query, CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortiesAsync(cancellationToken);
    }
}