using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Repositories.InMemory;

public sealed class InMemorySortiesRepository : ISortiesRepository
{
    private readonly IDictionary<Guid, Sortie> _sorties;

    private InMemorySortiesRepository()
    {
        _sorties = new Dictionary<Guid, Sortie>();
    }

    public static InMemorySortiesRepository CreerRepositoryVide() => new();

    public InMemorySortiesRepository AjouterSortie(Sortie sortie)
    {
        _sorties[sortie.Id] = sortie;
        return this;
    }

    public async Task<IEnumerable<Sortie>> GetSorties(CancellationToken cancellationToken)
    {
        return _sorties.Values;
    }

    public async Task<Sortie?> GetSortieParId(Guid id, CancellationToken cancellationToken)
    {
        return _sorties.TryGetValue(id, out var sortie) ? sortie : null;
    }

    public async Task PersisterSortie(Sortie sortie, CancellationToken cancellationToken)
    {
        _sorties[sortie.Id] = sortie;
    }
}
