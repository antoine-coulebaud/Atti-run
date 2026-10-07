using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AttiRun.WebApp.Repositories;

public class ParticipantsRepository(AttiRunDb dbContext) : IParticipantsRepository
{
    public async Task<Participant?> GetParticipantParNom(string nom, CancellationToken cancellationToken)
    {
        return await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nom, cancellationToken);
    }
}