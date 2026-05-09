using Mediator_REST_API.Application.Contracts.Repositories;
//using Mediator_REST_API.Application.Utilities;
using Mediator_REST_API.Domain.Entities;
using MediatR;

namespace Mediator_REST_API.Application.Features.DentalOffices.Queries.GetDentalOfficeDetail;

public class GetDentalOfficeDetailQueryHandler : IRequestHandler<GetDentalOfficeDetailQuery, DentalOfficeDetailDto>
{
    private readonly IDentalOfficeRepository _repository;

    public GetDentalOfficeDetailQueryHandler(IDentalOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<DentalOfficeDetailDto> Handle(GetDentalOfficeDetailQuery request, CancellationToken cancellationToken)
    {
        var dentalOffice = await _repository.GetById(request.Id);

        if (dentalOffice is null)
        {
            throw new KeyNotFoundException($"DentalOffice with Id {request.Id} was not found.");
        }

        return dentalOffice.ToDto();
    }
}