using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace AttiRun.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.AsScoped(), typeof(ApplicationDependencyInjection).Assembly);

        return services;
    }
}
