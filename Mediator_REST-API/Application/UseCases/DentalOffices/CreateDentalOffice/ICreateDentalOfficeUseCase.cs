using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;

public interface ICreateDentalOfficeUseCase
{
    Task<CreateDentalOfficeOutput> ExecuteAsync(CreateDentalOfficeInput input, CancellationToken cancellationToken = default);
}