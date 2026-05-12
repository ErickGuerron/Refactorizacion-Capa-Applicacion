using Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.AddDentalOffice.Dto;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mediator_REST_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DentalOfficeController : ControllerBase
{
    private readonly IAddDentalOfficeUseCase _addDentalOfficeUseCase;
    private readonly IGetDentalOfficeByIdUseCase _getDentalOfficeByIdUseCase;

    public DentalOfficeController(
        IAddDentalOfficeUseCase addDentalOfficeUseCase,
        IGetDentalOfficeByIdUseCase getDentalOfficeByIdUseCase)
    {
        _addDentalOfficeUseCase = addDentalOfficeUseCase;
        _getDentalOfficeByIdUseCase = getDentalOfficeByIdUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] AddDentalOfficeInput input,
        CancellationToken cancellationToken)
    {
        var output = await _addDentalOfficeUseCase.ExecuteAsync(input, cancellationToken);
        return Ok(output);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var output = await _getDentalOfficeByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(output);
    }
}
