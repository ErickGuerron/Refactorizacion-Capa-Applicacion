using FluentValidation;
using Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice;

public class AddDentalOfficeValidator : AbstractValidator<AddDentalOfficeInput>
{
    public AddDentalOfficeValidator()
    {
        RuleFor(input => input.Name)
            .NotNull().WithMessage("El campo Nombre es requerido")
            .NotEmpty().WithMessage("El campo Nombre es requerido");
    }
}
