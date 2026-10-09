using AttiRun.Application.Contracts.Repositories;
using AttiRun.Domain.Entities;
using MediatR;

namespace AttiRun.Application.Queries;

public record GetSortiesQuery : IRequest<IEnumerable<Sortie>>;

public class GetSortiesQueryHandler(ISortiesRepository sortiesRepository) : IRequestHandler<GetSortiesQuery, IEnumerable<Sortie>>
{
    public Task<IEnumerable<Sortie>> Handle(GetSortiesQuery query, CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortiesAsync(cancellationToken);
    }
}