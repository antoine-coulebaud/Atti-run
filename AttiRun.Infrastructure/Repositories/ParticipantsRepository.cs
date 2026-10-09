using AttiRun.Application.Contracts.Repositories;
using AttiRun.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttiRun.Infrastructure.Repositories;

public class ParticipantsRepository(AttiRunDb dbContext) : IParticipantsRepository
{
    public async Task<Participant?> GetParticipantParNom(string nom, CancellationToken cancellationToken)
    {
        return await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nom, cancellationToken);
    }
}