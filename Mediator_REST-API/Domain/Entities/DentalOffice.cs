namespace Mediator_REST_API.Domain.Entities;

public class DentalOffice
{
    public Guid Id { get; }

    public string Name { get; }

    public DentalOffice(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
