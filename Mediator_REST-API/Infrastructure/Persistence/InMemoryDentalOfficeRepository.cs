using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Infrastructure.Persistence;

public class InMemoryDentalOfficeRepository : IDentalOfficeRepository
{
    private static readonly List<DentalOffice> Offices = [];

    public Task<DentalOffice> Add(DentalOffice dentalOffice)
    {
        Offices.Add(dentalOffice);
        return Task.FromResult(dentalOffice);
    }

    public Task<DentalOffice?> GetById(Guid id)
    {
        var office = Offices.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(office);
    }
}
