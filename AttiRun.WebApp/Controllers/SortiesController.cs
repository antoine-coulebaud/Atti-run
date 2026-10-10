using AttiRun.Application.Commandes;
using AttiRun.Application.Queries;
using AttiRun.WebApp.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AttiRun.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SortiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<SortieDto>> GetSorties(CancellationToken cancellationToken)
    {
        var query = new GetSortiesQuery();
        var sorties = await mediator.Send(query, cancellationToken);

        return sorties.Select(s => s.ToDto());
    }

    [HttpGet("{id:guid}")]
    public async Task<Results<Ok<SortieDto>, NotFound>> GetSortieParId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSortieParIdQuery(id);
        var sortie = await mediator.Send(query, cancellationToken);

        return sortie is not null
            ? TypedResults.Ok(sortie.ToDto())
            : TypedResults.NotFound();
    }

    [HttpPost("{id:guid}/participer")]
    public async Task<Results<Ok<SortieDto>, NotFound, BadRequest<string>>> ParticiperSortie(
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
            return TypedResults.Ok(sortie.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/inviter")]
    public async Task<Results<Ok<SortieDto>, NotFound, BadRequest<string>>> InviterParticipant(
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
            return TypedResults.Ok(sortie.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}