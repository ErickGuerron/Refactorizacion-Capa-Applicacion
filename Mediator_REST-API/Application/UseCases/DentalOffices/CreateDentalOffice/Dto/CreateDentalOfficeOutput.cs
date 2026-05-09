namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;

public class CreateDentalOfficeOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}