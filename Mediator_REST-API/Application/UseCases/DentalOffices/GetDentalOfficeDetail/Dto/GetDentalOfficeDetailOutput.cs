namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;

public class GetDentalOfficeDetailOutput
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}