using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Contracts.Repositories;

public interface IParticipantsRepository
{
    Task<Participant?> GetParticipantParNom(string nom, CancellationToken cancellationToken);
}
