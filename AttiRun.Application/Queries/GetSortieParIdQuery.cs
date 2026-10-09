using AttiRun.Application.Contracts.Repositories;
using AttiRun.Domain.Entities;
using MediatR;

namespace AttiRun.Application.Queries;

public record GetSortieParIdQuery(Guid SortieId) : IRequest<Sortie?>;

public class GetSortieParIdQueryHandler(ISortiesRepository sortiesRepository) : IRequestHandler<GetSortieParIdQuery, Sortie?>
{
    public Task<Sortie?> Handle(GetSortieParIdQuery query, CancellationToken cancellationToken)
    {
        return sortiesRepository.GetSortieParIdAsync(query.SortieId, cancellationToken);
    }
}