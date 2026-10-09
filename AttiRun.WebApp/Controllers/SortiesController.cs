using AttiRun.WebApp.Contracts.Repositories;
using AttiRun.WebApp.Models;
using AttiRun.WebApp.Usecases;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AttiRun.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SortiesController(ISortiesRepository sortiesRepository, IParticipantsRepository participantsRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Sortie>> GetSorties(CancellationToken cancellationToken)
    {
        var usecase = new GetSortiesUsecase(sortiesRepository);
        return await usecase.HandleAsync(cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<Results<Ok<Sortie>, NotFound>> GetSortieParId(Guid id, CancellationToken cancellationToken)
    {
        var usecase = new GetSortieParIdUsecase(id, sortiesRepository);
        var sortie = await usecase.HandleAsync(cancellationToken);

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

        var usecase = new InscrireParticipantUsecase(id, nom, sortiesRepository, participantsRepository);
        try
        {
            var sortie = await usecase.HandleAsync(cancellationToken);
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

        var usecase = new InviterParticipantUsecase(id, nom, nomInvite, sortiesRepository, participantsRepository);
        try
        {
            var sortie = await usecase.HandleAsync(cancellationToken);
            return TypedResults.Ok(sortie);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}