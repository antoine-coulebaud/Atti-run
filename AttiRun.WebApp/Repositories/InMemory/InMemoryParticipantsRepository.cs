using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;

namespace AttiRun.WebApp.Repositories.InMemory;

public sealed class InMemoryParticipantsRepository : IParticipantsRepository
{
    private readonly IDictionary<Guid, Participant> _participants;

    private InMemoryParticipantsRepository()
    {
        _participants = new Dictionary<Guid, Participant>();
    }

    public static InMemoryParticipantsRepository CreerRepositoryVide() => new();
    
    public InMemoryParticipantsRepository AjouterParticipant(Participant participant)
    {
        _participants[participant.Id] = participant;
        return this;
    }

    public Task<Participant?> GetParticipantParNom(string nom, CancellationToken cancellationToken)
    {
        return Task.FromResult(_participants.Values.FirstOrDefault(p => p.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase)));
    }
}
