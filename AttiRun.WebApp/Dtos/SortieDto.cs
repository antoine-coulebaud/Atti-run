namespace AttiRun.WebApp.Dtos;

public record SortieDto(
    Guid Id,
    string Nom,
    string Localisation,
    DateTime Date,
    int NombreParticipantsMax,
    StatutSortieDto Statut,
    IReadOnlyCollection<ParticipantDto> Participants,
    IReadOnlyCollection<InvitationDto> Invitations);
