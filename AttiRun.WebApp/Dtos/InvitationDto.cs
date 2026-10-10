namespace AttiRun.WebApp.Dtos;

public record InvitationDto(Guid Id, ParticipantDto Invite, ParticipantDto Inviteur, StatutInvitationDto Statut);
