using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;

public interface IGetDentalOfficeDetailUseCase
{
    Task<GetDentalOfficeDetailOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}