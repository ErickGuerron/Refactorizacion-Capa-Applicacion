using Mediator_REST_API.Application.Contracts.Persistence;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Domain.Entities;
using MediatR;

namespace Mediator_REST_API.Application.Features.DentalOffices.Commands.CreateDentalOffice;

public class CreateDentalOfficeCommandHandler : IRequestHandler<CreateDentalOfficeCommand, Guid>
{
    private readonly IDentalOfficeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDentalOfficeCommandHandler(IDentalOfficeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateDentalOfficeCommand command, CancellationToken cancellationToken)
    {
        var dentalOffice = new DentalOffice(command.Name);

        try
        {
            var result = await _repository.Add(dentalOffice);
            await _unitOfWork.Commit();
            return result.Id;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
