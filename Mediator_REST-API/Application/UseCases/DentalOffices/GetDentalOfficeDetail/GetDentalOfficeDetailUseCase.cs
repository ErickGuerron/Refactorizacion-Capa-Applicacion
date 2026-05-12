using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.Exceptions;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;
using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;

public class GetDentalOfficeDetailUseCase : IGetDentalOfficeDetailUseCase
{
    private readonly IDentalOfficeRepository _repository;

    public GetDentalOfficeDetailUseCase(IDentalOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDentalOfficeDetailOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty", nameof(id));
        }

        var dentalOffice = await _repository.GetById(id);

        if (dentalOffice is null)
        {
            throw new EntityNotFoundException("DentalOffice", id);
        }

        return new GetDentalOfficeDetailOutput
        {
            Id = dentalOffice.Id,
            Name = dentalOffice.Name
        };
    }
}