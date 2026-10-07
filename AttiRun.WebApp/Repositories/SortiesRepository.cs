using AttiRun.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AttiRun.WebApp.Repositories;

public class SortiesRepository(AttiRunDb dbContext)
{
    public async Task<IEnumerable<Sortie>> GetSorties(CancellationToken cancellationToken)
    {
        return await dbContext.Sorties.ToListAsync(cancellationToken);
    }

    public async Task<Sortie?> GetSortieParId(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Sorties
            .Include(s => s.Invitations)
                .ThenInclude(i => i.Invite)
            .Include(s => s.Invitations)
                .ThenInclude(i => i.Inviteur)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task PersisterSortie(Sortie sortie, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(sortie).State == EntityState.Detached)
        {
            dbContext.Sorties.Add(sortie);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
