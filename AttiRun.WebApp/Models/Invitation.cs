namespace AttiRun.WebApp.Models;

public class Invitation
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Participant Invite { get; set; } = null!;
    public Participant Inviteur { get; set; } = null!;
    public StatutInvitation Statut { get; } = StatutInvitation.EnAttente;
}
