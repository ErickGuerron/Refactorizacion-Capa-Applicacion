//using Mediator_REST_API.Application.Utilities;
using MediatR;

namespace Mediator_REST_API.Application.Features.DentalOffices.Queries.GetDentalOfficeDetail;

public class GetDentalOfficeDetailQuery: IRequest<DentalOfficeDetailDto>
{
    public required Guid Id { get; set; }
}
