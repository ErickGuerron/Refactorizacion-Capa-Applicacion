using Mediator_REST_API.Application.Features.DentalOffices.Commands.CreateDentalOffice;
using Mediator_REST_API.Application.Features.DentalOffices.Queries.GetDentalOfficeDetail;
using Mediator_REST_API.Application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Mediator_REST_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DentalOfficeController : ControllerBase
{
    private readonly IMediator _mediator;

    public DentalOfficeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDentalOfficeCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetDentalOfficeDetailQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
