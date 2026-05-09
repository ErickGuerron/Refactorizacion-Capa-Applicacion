using FluentValidation;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator_REST_API.Application;

public static class RegisterApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Use Cases
        services.AddScoped<ICreateDentalOfficeUseCase, CreateDentalOfficeUseCase>();
        services.AddScoped<IGetDentalOfficeDetailUseCase, GetDentalOfficeDetailUseCase>();

        // Validators
        services.AddScoped<CreateDentalOfficeValidator>();

        return services;
    }
}