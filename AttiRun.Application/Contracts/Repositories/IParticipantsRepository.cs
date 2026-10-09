using AttiRun.Domain.Entities;

namespace AttiRun.Application.Contracts.Repositories;

public interface IParticipantsRepository
{
    Task<Participant?> GetParticipantParNom(string nom, CancellationToken cancellationToken);
}
