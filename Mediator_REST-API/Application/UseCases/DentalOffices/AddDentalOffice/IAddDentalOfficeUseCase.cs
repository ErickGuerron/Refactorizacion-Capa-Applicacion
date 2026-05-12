using Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice;

public interface IAddDentalOfficeUseCase
{
    Task<AddDentalOfficeOutput> ExecuteAsync(AddDentalOfficeInput input, CancellationToken cancellationToken = default);
}
