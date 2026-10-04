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
}