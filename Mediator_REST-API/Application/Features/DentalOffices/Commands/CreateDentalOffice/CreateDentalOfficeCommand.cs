//using Mediator_REST_API.Application.Utilities;
using MediatR;

public class CreateDentalOfficeCommand : IRequest<Guid>
{
    public required string Name { get; set; }
}
