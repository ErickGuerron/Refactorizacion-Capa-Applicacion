using FluentValidation;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;

public class CreateDentalOfficeValidator : AbstractValidator<CreateDentalOfficeInput>
{
    public CreateDentalOfficeValidator()
    {
        RuleFor(input => input.Name)
            .NotEmpty().WithMessage("El campo Nombre es requerido");
    }
}