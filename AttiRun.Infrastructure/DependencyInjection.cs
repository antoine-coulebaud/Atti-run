using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AttiRun.Application.Contracts.Repositories;
using AttiRun.Infrastructure.Repositories;

namespace AttiRun.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AttiRunDb>(options => options.UseInMemoryDatabase("atti-run-db"));

        services.AddScoped<ISortiesRepository, SortiesRepository>();
        services.AddScoped<IParticipantsRepository, ParticipantsRepository>();

        return services;
    }
}
