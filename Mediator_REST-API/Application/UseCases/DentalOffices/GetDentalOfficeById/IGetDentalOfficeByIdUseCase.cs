using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById;

public interface IGetDentalOfficeByIdUseCase
{
    Task<GetDentalOfficeByIdOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
