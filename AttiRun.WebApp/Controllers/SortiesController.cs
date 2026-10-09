using AttiRun.WebApp.Commandes;
using AttiRun.WebApp.Models;
using AttiRun.WebApp.Queries;
using AttiRun.WebApp.Usecases;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AttiRun.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SortiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Sortie>> GetSorties(CancellationToken cancellationToken)
    {
        var query = new GetSortiesQuery();
        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<Results<Ok<Sortie>, NotFound>> GetSortieParId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSortieParIdQuery(id);
        var sortie = await mediator.Send(query, cancellationToken);

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

        var command = new InscrireParticipantCommand(id, nom);
        try
        {
            var sortie = await mediator.Send(command, cancellationToken);
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

        var command = new InviterParticipantCommand(id, nom, nomInvite);
        try
        {
            var sortie = await mediator.Send(command, cancellationToken);
            return TypedResults.Ok(sortie);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}