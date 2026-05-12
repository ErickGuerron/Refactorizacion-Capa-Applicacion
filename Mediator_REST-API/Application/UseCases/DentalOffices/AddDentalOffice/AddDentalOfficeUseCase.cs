using FluentValidation;
using Mediator_REST_API.Application.Contracts.Persistence;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.Exceptions;
using Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice.Dto;
using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice;

public class AddDentalOfficeUseCase : IAddDentalOfficeUseCase
{
    private readonly IDentalOfficeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddDentalOfficeInput> _validator;

    public AddDentalOfficeUseCase(
        IDentalOfficeRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<AddDentalOfficeInput> validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<AddDentalOfficeOutput> ExecuteAsync(AddDentalOfficeInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new CustomValidationException(validationResult);
        }

        var dentalOffice = new DentalOffice(input.Name!);

        try
        {
            await _repository.Add(dentalOffice);
            await _unitOfWork.Commit();

            return new AddDentalOfficeOutput
            {
                Id = dentalOffice.Id,
                Name = dentalOffice.Name
            };
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
