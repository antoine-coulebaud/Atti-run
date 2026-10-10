using AttiRun.Domain.Entities;

namespace AttiRun.WebApp.Dtos;

public static class DtoMappings
{
    public static SortieDto ToDto(this Sortie sortie) => new(
        sortie.Id,
        sortie.Nom,
        sortie.Localisation,
        sortie.Date,
        sortie.NombreParticipantsMax,
        sortie.Statut.ToDto(),
        [.. sortie.Participants.Select(p => p.ToDto())],
        [.. sortie.Invitations.Select(i => i.ToDto())]);

    public static ParticipantDto ToDto(this Participant participant) => new(participant.Id, participant.Nom);

    public static InvitationDto ToDto(this Invitation invitation) => new(
        invitation.Id,
        invitation.Invite.ToDto(),
        invitation.Inviteur.ToDto(),
        invitation.Statut.ToDto());

    public static StatutSortieDto ToDto(this StatutSortie statut) => statut switch
    {
        StatutSortie.Ouverte => StatutSortieDto.Ouverte,
        StatutSortie.Annulee => StatutSortieDto.Annulee,
        _ => throw new ArgumentOutOfRangeException(nameof(statut), statut, "Statut de sortie inconnu."),
    };

    public static StatutInvitationDto ToDto(this StatutInvitation statut) => statut switch
    {
        StatutInvitation.EnAttente => StatutInvitationDto.EnAttente,
        StatutInvitation.Acceptee => StatutInvitationDto.Acceptee,
        StatutInvitation.Refusee => StatutInvitationDto.Refusee,
        _ => throw new ArgumentOutOfRangeException(nameof(statut), statut, "Statut d'invitation inconnu."),
    };
}
