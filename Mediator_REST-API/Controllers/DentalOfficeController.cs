using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mediator_REST_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DentalOfficeController : ControllerBase
{
    private readonly ICreateDentalOfficeUseCase _createDentalOfficeUseCase;
    private readonly IGetDentalOfficeDetailUseCase _getDentalOfficeDetailUseCase;

    public DentalOfficeController(
        ICreateDentalOfficeUseCase createDentalOfficeUseCase,
        IGetDentalOfficeDetailUseCase getDentalOfficeDetailUseCase)
    {
        _createDentalOfficeUseCase = createDentalOfficeUseCase;
        _getDentalOfficeDetailUseCase = getDentalOfficeDetailUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDentalOfficeInput input,
        CancellationToken cancellationToken)
    {
        var output = await _createDentalOfficeUseCase.ExecuteAsync(input, cancellationToken);
        return Ok(output);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var output = await _getDentalOfficeDetailUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(output);
    }
}