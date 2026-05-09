# Design — Refactor Application Layer from CQRS to Use Cases

## Technical Approach

Replace MediatR-based CQRS pattern with explicit Use Case classes. Each business operation gets its own dedicated interface (`ICreateDentalOfficeUseCase`, `IGetDentalOfficeDetailUseCase`) and implementation. Controllers depend directly on use case interfaces, not on MediatR abstractions.

---

## Architecture Decisions

### Decision: Use Case Interface Naming

**Choice**: `I{Operation}UseCase` with `ExecuteAsync` method

**Alternatives considered**: `I{Operation}Handler`, `I{Operation}Service`

**Rationale**: Clear intent — these are application-specific business operations, not generic handlers. `UseCase` suffix makes the pattern explicit in code reviews.

---

### Decision: Input/Output DTOs — solo cuando son necesarios

**Choice**: Solo Input DTO para operaciones que reciben request body (Create). Get recibe parámetros directos de la ruta.

**Alternatives considered**: Input/Output para todos los Use Cases

**Rationale**: Para operaciones GET, el parámetro viene de la URL (ruta, query string). Un wrapper Input DTO sería abstracción artificial. El binding de ASP.NET Core ya convierte el string a Guid.

**Regla**:
- Create → SÍ necesita Input DTO (valida request body)
- Get → NO necesita Input DTO (parámetro directo de la ruta)
- Update/Delete → depende de la complejidad

---

### Decision: Validation Integration

**Choice**: Inject `AbstractValidator<T>` directly into use case constructor

**Alternatives considered**: Service locator, validator factory

**Rationale**: Direct injection is explicit, testable, and follows DI best practices. FluentValidation's `ValidateAsync` is called manually within the use case.

---

### Decision: Exception Handling

**Choice**: `KeyNotFoundException` for missing entities, re-throw validation exceptions

**Alternatives considered**: `Result<T>` pattern, custom domain exceptions

**Rationale**: Standard .NET exceptions are already used by the infrastructure layer. Adding a `Result` pattern would increase surface area without adding value in this small codebase.

---

## Data Flow

```
Controller
    │ (injects ICreateDentalOfficeUseCase)
    ▼
ICreateDentalOfficeUseCase.ExecuteAsync(CreateDentalOfficeInput)
    │ (validates with FluentValidation)
    ▼
    ├─ validation fail → throws CustomValidationException
    │
    ▼
CreateDentalOffice entity
    │
    ▼ (add to IDentalOfficeRepository)
InMemoryDentalOfficeRepository
    │
    ▼ (commit IUnitOfWork)
InMemoryUnitOfWork.Commit()
    │
    ▼
CreateDentalOfficeOutput { Id, Name }
```

---

## File Changes

| File | Action | Description |
|------|--------|-------------|
| `Application/UseCases/DentalOffices/CreateDentalOffice/Dto/CreateDentalOfficeInput.cs` | Create | Input DTO with Name property |
| `Application/UseCases/DentalOffices/CreateDentalOffice/Dto/CreateDentalOfficeOutput.cs` | Create | Output DTO with Id, Name |
| `Application/UseCases/DentalOffices/CreateDentalOffice/ICreateDentalOfficeUseCase.cs` | Create | Interface with ExecuteAsync |
| `Application/UseCases/DentalOffices/CreateDentalOffice/CreateDentalOfficeUseCase.cs` | Create | Implementation with validation, repo, UoW |
| `Application/UseCases/DentalOffices/GetDentalOfficeDetail/Dto/GetDentalOfficeDetailOutput.cs` | Create | Output DTO with Id, Name |
| `Application/UseCases/DentalOffices/GetDentalOfficeDetail/IGetDentalOfficeDetailUseCase.cs` | Create | Interface with ExecuteAsync(Guid id) — NO Input DTO |
| `Application/UseCases/DentalOffices/GetDentalOfficeDetail/GetDentalOfficeDetailUseCase.cs` | Create | Implementation with repository — receives Guid directly |
| `Controllers/DentalOfficeController.cs` | Modify | Replace IMediator with use case interfaces |
| `Application/RegisterApplicationServices.cs` | Modify | Register use cases, remove MediatR |

---

## DTOs

### CreateDentalOfficeInput.cs

```csharp
namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;

/// <summary>
/// Input for creating a new dental office.
/// </summary>
public class CreateDentalOfficeInput
{
    /// <summary>
    /// Name of the dental office. Required and must not be empty.
    /// </summary>
    public required string Name { get; set; }
}
```

---

### CreateDentalOfficeOutput.cs

```csharp
namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;

/// <summary>
/// Output returned after successfully creating a dental office.
/// </summary>
public class CreateDentalOfficeOutput
{
    /// <summary>
    /// Unique identifier of the created dental office.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the created dental office.
    /// </summary>
    public required string Name { get; set; }
}
```

---

### GetDentalOfficeDetailInput.cs

```csharp
namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;

/// <summary>
/// Input for retrieving dental office details.
/// </summary>
public class GetDentalOfficeDetailInput
{
    /// <summary>
    /// Unique identifier of the dental office.
    /// </summary>
    public Guid Id { get; set; }
}
```

---

### GetDentalOfficeDetailOutput.cs

```csharp
namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;

/// <summary>
/// Output containing dental office details.
/// </summary>
public class GetDentalOfficeDetailOutput
{
    /// <summary>
    /// Unique identifier of the dental office.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the dental office.
    /// </summary>
    public required string Name { get; set; }
}
```

---

## Interfaces

### ICreateDentalOfficeUseCase.cs

```csharp
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;

/// <summary>
/// Use case for creating a new dental office.
/// </summary>
public interface ICreateDentalOfficeUseCase
{
    /// <summary>
    /// Executes the create dental office use case.
    /// </summary>
    /// <param name="input">Input data for creating the dental office.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Output containing the created dental office's Id and Name.</returns>
    Task<CreateDentalOfficeOutput> ExecuteAsync(
        CreateDentalOfficeInput input,
        CancellationToken cancellationToken = default);
}
```

---

### IGetDentalOfficeDetailUseCase.cs

```csharp
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;

/// <summary>
/// Use case for retrieving dental office details.
/// </summary>
public interface IGetDentalOfficeDetailUseCase
{
    /// <summary>
    /// Executes the get dental office detail use case.
    /// </summary>
    /// <param name="input">Input containing the dental office Id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Output containing the dental office details.</returns>
    Task<GetDentalOfficeDetailOutput> ExecuteAsync(
        GetDentalOfficeDetailInput input,
        CancellationToken cancellationToken = default);
}
```

---

## Use Case Implementations

### CreateDentalOfficeUseCase.cs

```csharp
using FluentValidation;
using Mediator_REST_API.Application.Contracts.Persistence;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.Exceptions;
using Mediator_REST_API.Application.Features.DentalOffices.Commands.CreateDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;
using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;

public class CreateDentalOfficeUseCase : ICreateDentalOfficeUseCase
{
    private readonly IDentalOfficeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateDentalOfficeCommandValidator _validator;

    public CreateDentalOfficeUseCase(
        IDentalOfficeRepository repository,
        IUnitOfWork unitOfWork,
        CreateDentalOfficeCommandValidator validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CreateDentalOfficeOutput> ExecuteAsync(
        CreateDentalOfficeInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateDentalOfficeCommand { Name = input.Name };
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new CustomValidationException(validationResult);
        }

        var dentalOffice = new DentalOffice(input.Name);

        try
        {
            await _repository.Add(dentalOffice);
            await _unitOfWork.Commit();

            return new CreateDentalOfficeOutput
            {
                Id = dentalOffice.Id,
                Name = dentalOffice.Name
            };
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
```

---

### GetDentalOfficeDetailUseCase.cs

```csharp
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;
using Mediator_REST_API.Domain.Entities;

namespace Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;

public class GetDentalOfficeDetailUseCase : IGetDentalOfficeDetailUseCase
{
    private readonly IDentalOfficeRepository _repository;

    public GetDentalOfficeDetailUseCase(IDentalOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDentalOfficeDetailOutput> ExecuteAsync(
        GetDentalOfficeDetailInput input,
        CancellationToken cancellationToken = default)
    {
        if (input.Id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(input));
        }

        var dentalOffice = await _repository.GetById(input.Id);

        if (dentalOffice is null)
        {
            throw new KeyNotFoundException($"DentalOffice with Id {input.Id} was not found.");
        }

        return new GetDentalOfficeDetailOutput
        {
            Id = dentalOffice.Id,
            Name = dentalOffice.Name
        };
    }
}
```

---

## Controller Refactor

### DentalOfficeController.cs

```csharp
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
        var input = new GetDentalOfficeDetailInput { Id = id };
        var output = await _getDentalOfficeDetailUseCase.ExecuteAsync(input, cancellationToken);
        return Ok(output);
    }
}
```

---

## DI Registration

### RegisterApplicationServices.cs

```csharp
using FluentValidation;
using Mediator_REST_API.Application.Features.DentalOffices.Commands.CreateDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator_REST_API.Application;

public static class RegisterApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Use Cases
        services.AddScoped<ICreateDentalOfficeUseCase, CreateDentalOfficeUseCase>();
        services.AddScoped<IGetDentalOfficeDetailUseCase, GetDentalOfficeDetailUseCase>();

        // Validators
        services.AddScoped<CreateDentalOfficeCommandValidator>();

        return services;
    }
}
```

---

## Cleanup Plan

### Files to DELETE (after verification):

| Path | Reason |
|------|--------|
| `Application/Utilities/IRequest.cs` | MediatR stub — not used |
| `Application/Utilities/IRequestHandler.cs` | MediatR stub — not used |
| `Application/Utilities/IMediator.cs` | MediatR stub — not used |
| `Application/Utilities/SimpleMediator.cs` | MediatR stub — not used |
| `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommand.cs` | Replaced by Use Case |
| `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommandHandler.cs` | Replaced by Use Case |
| `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommandValidator.cs` | Keep — needed by CreateDentalOfficeUseCase |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/GetDentalOfficeDetailQuery.cs` | Replaced by Use Case |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/GetDentalOfficeDetailQueryHandler.cs` | Replaced by Use Case |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/DentalOfficeDetailDto.cs` | Replaced by Use Case DTOs |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/MapperExtensions.cs` | Logic moved to Use Case |

### Package reference to remove:
- `MediatR` from `.csproj` (if present as direct reference)

---

## Testing Strategy

| Layer | What to Test | Approach |
|-------|-------------|----------|
| Unit | CreateDentalOfficeUseCase validation logic | Mock validator, verify IsValid=false throws |
| Unit | CreateDentalOfficeUseCase success path | Mock repo + UoW, verify entity created and output returned |
| Unit | GetDentalOfficeDetailUseCase not found | Mock repo returning null, verify KeyNotFoundException |
| Unit | GetDentalOfficeDetailUseCase success | Mock repo, verify output mapped correctly |
| Integration | Full create flow | HTTP POST → UseCase → Repository → verify in-memory store |
| Integration | Full get flow | HTTP GET → UseCase → Repository → verify returned DTO |

---

## Migration / Rollout

No migration required. This is a pure refactor with no data or schema changes.

---

## Open Questions

None — all technical decisions are captured above.

---

## Design Principles Applied

- **DIP**: Controllers depend on `ICreateDentalOfficeUseCase` / `IGetDentalOfficeDetailUseCase` (abstractions), not on implementations
- **ISP**: Each interface has one method (`ExecuteAsync`) — clients only depend on what they use
- **SRP**: `CreateDentalOfficeUseCase` handles one operation; `GetDentalOfficeDetailUseCase` handles one operation
- **Clean Architecture**: Domain layer untouched; Application layer now has Use Cases; Infrastructure has repositories