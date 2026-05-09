using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.Features.DentalOffices.Queries.GetDentalOfficeDetail;

internal static class MapperExtensions
{
    public static  DentalOfficeDetailDto ToDto(this DentalOffice dentalOffice)
    {
        var dto = new DentalOfficeDetailDto
        {
            Id = dentalOffice.Id,
            Name = dentalOffice.Name,
        };

        return dto;
    }
}
