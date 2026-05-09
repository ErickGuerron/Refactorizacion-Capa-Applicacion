using Mediator_REST_API.Application.Features.DentalOffices.Commands.CreateDentalOffice;
using Mediator_REST_API.Application.Features.DentalOffices.Queries.GetDentalOfficeDetail;
using FluentValidation;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator_REST_API.Application;

public static class RegisterApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //services.AddTransient<IMediator, SimpleMediator>();

        //services.AddScoped<IRequestHandler<CreateDentalOfficeCommand, Guid>, CreateDentalOfficeCommandHandler>();
        //services.AddScoped<IRequestHandler<GetDentalOfficeDetailQuery, DentalOfficeDetailDto>, GetDentalOfficeDetailQueryHandler>();

        //services.AddValidatorsFromAssemblyContaining<CreateDentalOfficeCommandValidator>();

        //return services;

        //Registrar automaticamento todos os handlers e validadores usando MediatR e FluentValidation
        services.AddValidatorsFromAssemblies((IEnumerable<Assembly>)Assembly.GetExecutingAssembly());

        //Registrar todos los handlers 
        return services;
    }
}
