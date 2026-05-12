using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.Exceptions;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById.Dto;
using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById;

public class GetDentalOfficeByIdUseCase : IGetDentalOfficeByIdUseCase
{
    private readonly IDentalOfficeRepository _repository;

    public GetDentalOfficeByIdUseCase(IDentalOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDentalOfficeByIdOutput> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
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

        return new GetDentalOfficeByIdOutput
        {
            Id = dentalOffice.Id,
            Name = dentalOffice.Name
        };
    }
}
