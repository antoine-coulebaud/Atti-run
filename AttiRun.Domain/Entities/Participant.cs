namespace AttiRun.Domain.Entities;

public class Participant
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Nom { get; set; } = string.Empty;
}