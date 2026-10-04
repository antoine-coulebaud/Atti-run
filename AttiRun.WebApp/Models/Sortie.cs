namespace AttiRun.WebApp.Models;

public class Sortie
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Nom { get; set; } = string.Empty;
    public string Localisation { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int NombreParticipantsMax { get; set; }
    public StatutSortie Statut { get; set; }
    public ICollection<Participant> Participants { get; init; } = [];
}