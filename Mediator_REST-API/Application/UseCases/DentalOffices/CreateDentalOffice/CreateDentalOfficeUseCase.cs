using FluentValidation;
using Mediator_REST_API.Application.Contracts.Persistence;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.Exceptions;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;
using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;

public class CreateDentalOfficeUseCase : ICreateDentalOfficeUseCase
{
    private readonly IDentalOfficeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateDentalOfficeInput> _validator;

    public CreateDentalOfficeUseCase(
        IDentalOfficeRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateDentalOfficeInput> validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CreateDentalOfficeOutput> ExecuteAsync(CreateDentalOfficeInput input, CancellationToken cancellationToken = default)
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

            return new CreateDentalOfficeOutput
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