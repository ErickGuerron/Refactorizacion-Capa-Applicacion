using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.Contracts.Repositories;

public interface IDentalOfficeRepository
{
    Task<DentalOffice> Add(DentalOffice dentalOffice);
    Task<DentalOffice?> GetById(Guid id);
}
