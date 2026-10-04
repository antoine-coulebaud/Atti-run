using AttiRun.WebApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttiRun.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SortiesController(AttiRunDb dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Sortie>> GetAllSorties(CancellationToken cancellationToken)
    {
        return await dbContext.Sorties.ToListAsync(cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<Results<Ok<Sortie>, NotFound>> GetSortieById(Guid id, CancellationToken cancellationToken)
    {
        var sortie = await dbContext.Sorties.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return sortie is not null
            ? TypedResults.Ok(sortie)
            : TypedResults.NotFound();
    }

    [HttpPost("{id:guid}/participer")]
    public async Task<Results<Ok<Sortie>, NotFound, BadRequest<string>>> ParticiperSortie(
        Guid id,
        [FromHeader(Name = "x-username")] string? nom,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(nom))
        {
            return TypedResults.BadRequest("Le nom du participant est requis.");
        }

        var sortie = await dbContext.Sorties.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            
        if (sortie is null)
        {
            return TypedResults.NotFound();
        }

        var participant = await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nom, cancellationToken);
        if (participant is null)
        {
            return TypedResults.BadRequest("Participant non trouvé.");
        }

        if (sortie.Participants.Any(p => p.Id == participant.Id))
        {
            return TypedResults.BadRequest($"Le participant {participant.Nom} est déjà inscrit à cette sortie.");
        }

        if (sortie.Statut != StatutSortie.Ouverte)
        {
            return TypedResults.BadRequest("La sortie n'est pas ouverte aux inscriptions.");
        }

        if (sortie.Date < DateTime.Now)
        {
            return TypedResults.BadRequest("La sortie est déjà passée.");
        }

        if (sortie.Participants.Count >= sortie.NombreParticipantsMax)
        {
            return TypedResults.BadRequest("La sortie est complète.");
        }

        sortie.Participants.Add(participant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(sortie);
    }

    [HttpPost("{id:guid}/inviter")]
    public async Task<Results<Ok<Sortie>, NotFound, BadRequest<string>>> InviterParticipant(
        Guid id,
        [FromHeader(Name = "x-username")] string? nom,
        [FromBody] string nomInvite,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(nom))
        {
            return TypedResults.BadRequest("Nom du participant manquant.");
        }

        if (string.IsNullOrEmpty(nomInvite))
        {
            return TypedResults.BadRequest("Nom du participant à inviter manquant.");
        }

        var sortie = await dbContext.Sorties
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (sortie is null)
        {
            return TypedResults.NotFound();
        }

        var participant = await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nom, cancellationToken);
        if (participant is null)
        {
            return TypedResults.BadRequest("Participant non trouvé.");
        }

        var invite = await dbContext.Participants.FirstOrDefaultAsync(p => p.Nom == nomInvite, cancellationToken);
        if (invite is null)
        {
            return TypedResults.BadRequest("Participant à inviter non trouvé.");
        }

        if (sortie.Statut != StatutSortie.Ouverte)
        {
            return TypedResults.BadRequest("La sortie n'est pas ouverte aux inscriptions.");
        }

        if (sortie.Date < DateTime.Now)
        {
            return TypedResults.BadRequest("La sortie est déjà passée.");
        }

        if (sortie.Participants.Count >= sortie.NombreParticipantsMax)
        {
            return TypedResults.BadRequest("La sortie est complète.");
        }

        var invitation = new Invitation
        {
            Invite = invite,
            Inviteur = participant,
        };

        sortie.Invitations.Add(invitation);

        dbContext.Invitations.Add(invitation);
        dbContext.Update(sortie);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(sortie);
    }
}