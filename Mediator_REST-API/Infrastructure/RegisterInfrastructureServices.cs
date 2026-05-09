using Mediator_REST_API.Application.Contracts.Persistence;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator_REST_API.Infrastructure;

public static class RegisterInfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDentalOfficeRepository, InMemoryDentalOfficeRepository>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        return services;
    }
}
