using AttiRun.WebApp.Models;
using AttiRun.WebApp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttiRun.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SortiesController(AttiRunDb dbContext) : ControllerBase
{
    private readonly SortiesService sortiesService = new(dbContext);

    [HttpGet]
    public async Task<IEnumerable<Sortie>> GetAllSorties(CancellationToken cancellationToken)
    {
        return await dbContext.Sorties.ToListAsync(cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<Results<Ok<Sortie>, NotFound>> GetSortieById(Guid id, CancellationToken cancellationToken)
    {
        var sortie = await sortiesService.GetSortieParId(id, cancellationToken);

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

        var sortie = await sortiesService.GetSortieParId(id, cancellationToken);
        if (sortie is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            sortie = await sortiesService.InscrireParticipant(sortie, nom, cancellationToken);
            return TypedResults.Ok(sortie);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
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

        var sortie = await sortiesService.GetSortieParId(id, cancellationToken);
        if (sortie is null)
        {
            return TypedResults.NotFound();
        }

        try
        {
            sortie = await sortiesService.InviterParticipant(sortie, nom, nomInvite, cancellationToken);
            return TypedResults.Ok(sortie);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}