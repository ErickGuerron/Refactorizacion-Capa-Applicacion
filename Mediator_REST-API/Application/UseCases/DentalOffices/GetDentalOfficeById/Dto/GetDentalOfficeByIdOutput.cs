namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById.Dto;

public class GetDentalOfficeByIdOutput
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
