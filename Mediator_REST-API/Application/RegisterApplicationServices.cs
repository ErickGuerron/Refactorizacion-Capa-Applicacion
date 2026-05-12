using FluentValidation;
using Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice.Dto;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator_REST_API.Application;

public static class RegisterApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Use Cases
        services.AddScoped<IAddDentalOfficeUseCase, AddDentalOfficeUseCase>();
        services.AddScoped<IGetDentalOfficeByIdUseCase, GetDentalOfficeByIdUseCase>();

        // Validators
        services.AddScoped<IValidator<AddDentalOfficeInput>, AddDentalOfficeValidator>();

        return services;
    }
}
