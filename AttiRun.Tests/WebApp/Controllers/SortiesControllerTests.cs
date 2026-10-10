using System.Net;
using System.Net.Http.Json;
using AttiRun.Application.Queries;
using AttiRun.Domain.Entities;
using AttiRun.WebApp.Controllers;
using AttiRun.WebApp.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace AttiRun.Tests.WebApp.Controllers;

public sealed class SortiesControllerTests : IAsyncLifetime
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private IHost _host = null!;
    private HttpClient _client = null!;

    private readonly Sortie _sortie = new()
    {
        Nom = "Sortie test",
        Localisation = "Paris",
        Date = new DateTime(2027, 06, 20),
        NombreParticipantsMax = 10,
        Statut = StatutSortie.Ouverte,
    };

    public async Task InitializeAsync()
    {
        _host = await new HostBuilder()
            .ConfigureWebHost(webBuilder => webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddControllers().AddApplicationPart(typeof(SortiesController).Assembly);
                    services.AddSingleton(_mediator);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                }))
            .StartAsync();

        _client = _host.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }

    [Fact(DisplayName = "Récupérer une sortie existante retourne Ok avec la sortie")]
    public async Task GetSortieParId_SortieExistante_RetourneOkAvecLaSortie()
    {
        _mediator.Send(new GetSortieParIdQuery(_sortie.Id), Arg.Any<CancellationToken>()).Returns(_sortie);

        using var response = await _client.GetAsync(new Uri($"/api/sorties/{_sortie.Id}", UriKind.Relative));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sortieDto = await response.Content.ReadFromJsonAsync<SortieDto>();
        sortieDto.Should().BeEquivalentTo(_sortie.ToDto());
    }

    [Fact(DisplayName = "Récupérer une sortie inexistante retourne NotFound")]
    public async Task GetSortieParId_SortieInexistante_RetourneNotFound()
    {
        var id = Guid.NewGuid();
        _mediator.Send(new GetSortieParIdQuery(id), Arg.Any<CancellationToken>()).Returns((Sortie?)null);

        using var response = await _client.GetAsync(new Uri($"/api/sorties/{id}", UriKind.Relative));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _mediator.Received(1).Send(new GetSortieParIdQuery(id), Arg.Any<CancellationToken>());
    }
}
