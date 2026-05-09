using FluentValidation.Results;

namespace Mediator_REST_API.Application.Exceptions;

public class CustomValidationException : Exception
{
    public ValidationResult ValidationResult { get; }

    public CustomValidationException(ValidationResult validationResult)
        : base("Validation failed")
    {
        ValidationResult = validationResult;
    }
}
