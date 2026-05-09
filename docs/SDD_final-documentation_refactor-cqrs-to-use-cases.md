# Documentación Final — Refactorización de CQRS a Use Cases

## Cambio: `refactor-cqrs-to-use-cases`

---

## 1. Código Refactorizado y Ejecutando

### Archivos Nuevos Creados

```
Application/UseCases/DentalOffices/CreateDentalOffice/
├── CreateDentalOfficeInput.cs         → DTO de entrada (request body)
├── CreateDentalOfficeOutput.cs        → DTO de salida (response)
├── ICreateDentalOfficeUseCase.cs      → Interfaz del Use Case
├── CreateDentalOfficeUseCase.cs       → Implementación
├── CreateDentalOfficeValidator.cs     → FluentValidation
└── Dto/
    ├── CreateDentalOfficeInput.cs
    └── CreateDentalOfficeOutput.cs

Application/UseCases/DentalOffices/GetDentalOfficeDetail/
├── IGetDentalOfficeDetailUseCase.cs   → Interfaz del Use Case
├── GetDentalOfficeDetailUseCase.cs    → Implementación
└── Dto/
    └── GetDentalOfficeDetailOutput.cs  → DTO de salida (response)

Nota: GetDentalOfficeDetail NO tiene Input DTO porque el parámetro (Guid id)
viene directamente de la ruta URL — no requiere wrapper.
```

### Archivos Modificados

| Archivo | Cambio |
|---------|--------|
| `Controllers/DentalOfficeController.cs` | Reemplazado `IMediator` por inyección directa de use cases |
| `Application/RegisterApplicationServices.cs` | Registrados use cases como scoped services |

### Archivos Eliminados

| Archivo | Razón |
|---------|-------|
| `Application/Utilities/IRequest.cs` | MediatR stub — no usado |
| `Application/Utilities/IRequestHandler.cs` | MediatR stub — no usado |
| `Application/Utilities/IMediator.cs` | MediatR stub — no usado |
| `Application/Utilities/SimpleMediator.cs` | MediatR stub — no usado |
| `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommand.cs` | Reemplazado por Use Case |
| `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommandHandler.cs` | Reemplazado por Use Case |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/GetDentalOfficeDetailQuery.cs` | Reemplazado por Use Case |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/GetDentalOfficeDetailQueryHandler.cs` | Reemplazado por Use Case |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/DentalOfficeDetailDto.cs` | Reemplazado por DTOs del Use Case |
| `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/MapperExtensions.cs` | Lógica movida al Use Case |

### Tests Unitarios (7/7 pasando)

```
Mediator_REST_API.Tests/
├── UseCases/
│   ├── CreateDentalOffice/
│   │   └── CreateDentalOfficeUseCaseTests.cs (4 tests)
│   └── GetDentalOfficeDetail/
│       └── GetDentalOfficeDetailUseCaseTests.cs (3 tests)
```

| Test | Escenario | Resultado |
|------|-----------|-----------|
| `ExecuteAsync_WithValidInput_ReturnsOutputWithIdAndName` | Create con nombre válido | ✅ Passed |
| `ExecuteAsync_WithEmptyName_ThrowsCustomValidationException` | Create con nombre vacío | ✅ Passed |
| `ExecuteAsync_WithWhitespaceName_ThrowsCustomValidationException` | Create con espacios | ✅ Passed |
| `ExecuteAsync_OnRepositoryException_CallsRollback` | Create con excepción de repo | ✅ Passed |
| `ExecuteAsync_WithExistingId_ReturnsOutputWithIdAndName` | Get con ID existente | ✅ Passed |
| `ExecuteAsync_WithNonExistingId_ThrowsKeyNotFoundException` | Get con ID inexistente | ✅ Passed |
| `ExecuteAsync_WithEmptyGuid_ThrowsArgumentException` | Get con Guid.Empty | ✅ Passed |

### Verificación de Build

```
dotnet build Mediator_REST_API/Mediator_REST-API.csproj
→ Build succeeded. 0 Warning(s), 0 Error(s)

dotnet test Mediator_REST_API.Tests/Mediator_REST_API.Tests.csproj
→ Test Run Successful. Total tests: 7, Passed: 7
```

---

## 2. Nueva Estructura de Carpetas

### Estructura Final — Clean Architecture

```
Mediator_REST_API/
├── Application/
│   ├── UseCases/
│   │   └── DentalOffices/
│   │       ├── CreateDentalOffice/
│   │       │   ├── ICreateDentalOfficeUseCase.cs
│   │       │   ├── CreateDentalOfficeUseCase.cs
│   │       │   ├── CreateDentalOfficeValidator.cs
│   │       │   ├── CreateDentalOfficeInput.cs
│   │       │   └── CreateDentalOfficeOutput.cs
│   │       └── GetDentalOfficeDetail/
│   │           ├── IGetDentalOfficeDetailUseCase.cs
│   │           ├── GetDentalOfficeDetailUseCase.cs
│   │           └── Dto/
│   │               └── GetDentalOfficeDetailOutput.cs
│   │           (Sin Input DTO — el Guid viene directo de la ruta)
│   ├── Contracts/
│   │   ├── Repositories/
│   │   │   └── IDentalOfficeRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── Exceptions/
│   │   └── CustomValidationException.cs
│   └── RegisterApplicationServices.cs
├── Domain/
│   └── Entities/
│       └── DentalOffice.cs
├── Infrastructure/
│   ├── Persistence/
│   │   ├── InMemoryDentalOfficeRepository.cs
│   │   └── InMemoryUnitOfWork.cs
│   └── RegisterInfrastructureServices.cs
├── Controllers/
│   └── DentalOfficeController.cs
└── Program.cs
```

### Comparación: Estructura CQRS vs Use Cases

| Aspecto | CQRS (antes) | Use Cases (ahora) |
|----------|-------------|------------------|
| Carpetas principales | `Commands/` y `Queries/` | `UseCases/` |
| Nomenclatura | `CreateDentalOfficeCommand` + Handler | `CreateDentalOfficeUseCase` |
| Archivos por operación | 2-4 archivos (Command, Handler, Validator, DTO) | 4-5 archivos (Input, Output, Interface, Impl, Validator) |
| Profundidad de carpetas | `Features/DentalOffices/Commands/CreateDentalOffice/` | `UseCases/DentalOffices/CreateDentalOffice/` |
| Total de archivos Application | ~15 archivos | ~12 archivos |

---

## 3. Flujo de Ejecución

### CreateDentalOffice — Flujo Completo

```
HTTP POST /api/dentaloffice
         │
         ▼
DentalOfficeController.Create(CreateDentalOfficeInput)
         │ (inyecta ICreateDentalOfficeUseCase)
         ▼
ICreateDentalOfficeUseCase.ExecuteAsync(input)
         │
         ▼ (valida con CreateDentalOfficeValidator)
    ┌────┴────┐
    │ IsValid? │
    └────┬────┘
    No   │   Yes
    ▼    │    ▼
throw    │ CreateDentalOffice entity
CustomValidationException
         │
         ▼
IDentalOfficeRepository.Add(dentalOffice)
         │
         ▼
IUnitOfWork.Commit()
         │
         ▼
CreateDentalOfficeOutput { Id, Name }
         │
         ▼
HTTP 200 OK + { id, name }
```

### GetDentalOfficeDetail — Flujo Completo

```
HTTP GET /api/dentaloffice/{id}
         │
         ▼
DentalOfficeController.GetById(Guid id)
         │ (inyecta IGetDentalOfficeDetailUseCase)
         ▼
IGetDentalOfficeDetailUseCase.ExecuteAsync(Guid id)  ← Sin Input DTO
         │
         ▼
     ┌────┴────┐
     │ Id != Empty? │
     └────┬────┘
     No   │   Yes
     ▼    │    ▼
throw    │ IDentalOfficeRepository.GetById(id)
ArgumentException
         │
         ▼
     ┌────┴────┐
     │ Found?  │
     └────┬────┘
     No   │   Yes
     ▼    │    ▼
throw    │ GetDentalOfficeDetailOutput { Id, Name }
KeyNotFoundException
         │
         ▼
HTTP 200 OK + { id, name }  │  HTTP 404 Not Found
```

**Nota importante**: El Use Case de Get recibe `Guid id` directamente — no requiere Input DTO porque el parámetro viene de la ruta URL. Esto elimina abstracción artificial innecesaria.

---

## 4. Cambios Arquitectónicos Realizados

### Cambio 1: Eliminación de MediatR/CQRS

**Antes**:
```csharp
// Controller con MediatR
public class DentalOfficeController : ControllerBase
{
    private readonly IMediator _mediator;

    public DentalOfficeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDentalOfficeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
```

**Después**:
```csharp
// Controller con Use Cases
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
    public async Task<IActionResult> Create([FromBody] CreateDentalOfficeInput input)
    {
        var output = await _createDentalOfficeUseCase.ExecuteAsync(input);
        return Ok(output);
    }
}
```

### Cambio 2: De Commands/Queries a Use Cases

**Antes (CQRS)**:
```
Features/DentalOffices/Commands/CreateDentalOffice/
├── CreateDentalOfficeCommand.cs      → IRequest
├── CreateDentalOfficeCommandHandler.cs → IRequestHandler
└── CreateDentalOfficeCommandValidator.cs

Features/DentalOffices/Queries/GetDentalOfficeDetail/
├── GetDentalOfficeDetailQuery.cs      → IRequest
├── GetDentalOfficeDetailQueryHandler.cs → IRequestHandler
├── DentalOfficeDetailDto.cs
└── MapperExtensions.cs
```

**Después (Use Cases)**:
```
UseCases/DentalOffices/CreateDentalOffice/
├── ICreateDentalOfficeUseCase.cs      → Interfaz explícita
├── CreateDentalOfficeUseCase.cs       → Implementación
├── CreateDentalOfficeInput.cs          → DTO de entrada (request body)
├── CreateDentalOfficeOutput.cs         → DTO de salida (response)
└── CreateDentalOfficeValidator.cs     → FluentValidation

UseCases/DentalOffices/GetDentalOfficeDetail/
├── IGetDentalOfficeDetailUseCase.cs   → Interfaz explícita
├── GetDentalOfficeDetailUseCase.cs    → Implementación
└── Dto/
    └── GetDentalOfficeDetailOutput.cs → DTO de salida (response)
    (Sin Input DTO — el Guid viene directo de la ruta)
```

### Cambio 3: DI Registration

**Antes**:
```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDentalOfficeCommand).Assembly));
    return services;
}
```

**Después**:
```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Use Cases
    services.AddScoped<ICreateDentalOfficeUseCase, CreateDentalOfficeUseCase>();
    services.AddScoped<IGetDentalOfficeDetailUseCase, GetDentalOfficeDetailUseCase>();

    // Validators
    services.AddScoped<CreateDentalOfficeValidator>();

    return services;
}
```

---

## 5. Comparación: CQRS vs Application Services vs Use Cases

### Tabla Comparativa Completa

| Elemento | CQRS | Application Services | Use Cases |
|----------|------|---------------------|-----------|
| **Organización** | `Commands/` y `Queries/` | `Services/` | `UseCases/` |
| ** Clase central** | Un Handler por operación | Una clase con múltiples métodos | Una clase por caso de uso |
| **Interfaz** | `IRequest<T>` / `IRequestHandler<T,R>` | `IService` (métodos múltiples) | `I{Operation}UseCase` (un método) |
| **Mediador** | Necesario (`IMediator.Send()`) | Opcional | Opcional |
| **Método principal** | `Handle(TRequest)` | `CreateAsync()`, `GetByIdAsync()`, etc. | `ExecuteAsync(TInput)` |
| **DTOs** | Command + Handler DTO | Varios DTOs por servicio | Input/Output separados — solo cuando son necesarios (Create necesita Input, Get no) |
| **Popularidad .NET** | Muy alta (MediatR) | Alta (ASP.NET Core tradicional) | Alta (Clean Architecture) |
| **Referencia** | MediatR | Repository + Service pattern | Uncle Bob / Clean Architecture |
| **Testabilidad** | Alta (mocking de handlers) | Media (servicio grande) | Alta (clase pequeña y enfocada) |
| **Acoplamiento** | Bajo (mediador abstracción) | Medio (cliente conoce servicio) | Bajo (interfaz pequeña) |
| **Complejidad** | Media (framework necesario) | Baja (sin framework) | Baja (sin framework) |

### Cuándo usar cada uno

| Enfoque | Mejor para |
|---------|-----------|
| **CQRS** | Sistemas con múltiples operaciones complejas, necesidad de mediator pattern, pipelines de behaviors (validación, logging, caching) |
| **Application Services** | Sistemas tradicionales ASP.NET Core, operaciones relacionadas en un servicio,团队的 .NET clásico |
| **Use Cases** | Clean Architecture puro, sistemas pequeños a medianos, énfasis en testabilidad y claridad de intent |

---

## 6. Ventajas y Desventajas del Enfoque Use Cases

### Ventajas

| # | Ventaja | Descripción |
|---|---------|-------------|
| 1 | **Visibilidad de intent** | El nombre `CreateDentalOfficeUseCase` comunica exactamente qué hace el sistema |
| 2 | **Sin overhead de framework** | No requiere MediatR ni otro mediador — reduce dependencias |
| 3 | **Testabilidad** | Cada Use Case es una clase concreta con una responsabilidad — mocking directo |
| 4 | **Desacoplamiento** | El Controller depende de abstracciones pequeñas (`ICreateDentalOfficeUseCase`), no de un mediador genérico |
| 5 | **Single Responsibility** | Un Use Case = una operación de negocio — cada clase tiene una sola razón para cambiar |
| 6 | **Escalabilidad** | Agregar un nuevo caso de uso no requiere modificar una clase existente — se crea una nueva |
| 7 | **ISP Compliance** | Cada interfaz es pequeña y enfocada en una sola operación — clientes solo dependen de lo que usan |
| 8 | **Clean Architecture** | Alineado con los principios de Uncle Bob — los Use Cases son interactores del sistema |
| 9 | **Nomenclatura explícita** | `ExecuteAsync()` deja claro que es un caso de uso, no un servicio genérico |
| 10 | **Validación integrada** | El validator se inyecta directamente en el Use Case, sin pasar por un mediador |

### Desventajas

| # | Desventaja | Mitigación |
|---|-----------|-----------|
| 1 | **Más archivos** | Cada Use Case requiere 4-5 archivos vs 1 Command/Handler — tradeoff en claridad vs cantidad |
| 2 | **Namespace sprawl** | Estructura de carpetas más profunda (`UseCases/DentalOffices/CreateDentalOffice/`) — pero es consistente |
| 3 | **Duplicación de lógica de validación** | Si varios Use Cases validan lo mismo, puede haber repetición — mitigado con validadores reutilizables |
| 4 | **No hay pipeline de behaviors** | Sin MediatR, no hay forma de encadenar cross-cutting concerns automáticamente — mitigado con middleware de ASP.NET Core |
| 5 | **Sintaxis más verbose** | `useCase.ExecuteAsync(input)` vs `mediator.Send(command)` — pero es más explicativo |

---

## 7. Justificación Técnica de las Decisiones Tomadas

### Decisión 1: Use Cases sobre Application Services

**Razón**: En un Use Case, la responsabilidad de cada clase está definida por el nombre mismo. `CreateDentalOfficeUseCase` sabe crear consultorios dentales, y solo eso. En Application Services, `DentalOfficeService` podría crecer con el tiempo acumulando métodos relacionados pero distintos.

**Aplicación SOLID**:
- **SRP**: Cada Use Case tiene una responsabilidad única
- **ISP**: Cada interfaz tiene un solo método (`ExecuteAsync`)
- **DIP**: El Controller depende de abstracciones (`ICreateDentalOfficeUseCase`), no de implementaciones

### Decisión 2: Interfaces explícitas por Use Case

**Razón**: Cada Use Case tiene su propia interfaz (`ICreateDentalOfficeUseCase`, `IGetDentalOfficeDetailUseCase`). Esto permite:
- Mockear solo el Use Case que se necesita en tests
- Cambiar la implementación de un Use Case sin afectar otros
- Mantener las dependencias mínimas — cada Controller solo conoce los Use Cases que usa

**Alternativa considerada**: Una interfaz genérica `IUseCase<TInput, TOutput>` con una implementación genérica. **Descartada** porque pierde la semántica de negocio y hace más difícil el tracking de qué Use Cases existen.

### Decisión 3: Input/Output DTOs — solo cuando son necesarios

**Regla aplicada**:
- **Create**: SÍ necesita Input DTO → valida el request body
- **Get**: NO necesita Input DTO → el parámetro (Guid id) viene directo de la ruta URL

**Razón de esta decisión**: El `GetDentalOfficeDetailInput` era una abstracción artificial. El Guid ya viene bound desde la URL via ASP.NET Core model binding. Un wrapper no aporta nada.

**Situaciones donde SÍ tendría sentido un Input DTO en Get**:
- Si el GET necesitara múltiples parámetros (filtros, pagination, sorting)
- Si necesitara validación compleja del request
- Si el ID fuera compuesto o necesitara transformación

**Resultado**:
- `CreateDentalOfficeInput` → existe (valida el body)
- `CreateDentalOfficeOutput` → existe (define response)
- `GetDentalOfficeDetailInput` → **eliminado** (no necesario)
- `GetDentalOfficeDetailOutput` → existe (define response)

```csharp
// Create — SÍ usa Input DTO (valida request body)
Task<CreateDentalOfficeOutput> ExecuteAsync(CreateDentalOfficeInput input, ...)

// Get — NO usa Input DTO (parámetro directo de la URL)
Task<GetDentalOfficeDetailOutput> ExecuteAsync(Guid id, ...)

### Decisión 4: Validator inyectado via constructor

**Razón**: El validador (`CreateDentalOfficeValidator`) se inyecta en el constructor del Use Case. Esto sigue DI best practices:
- La dependencia es explícita
- Se puede mockear en tests
- No hay service locator ni dependencia oculta

```csharp
public CreateDentalOfficeUseCase(
    IDentalOfficeRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<CreateDentalOfficeInput> validator) // explícito
```

### Decisión 5: Excepciones estándar de .NET

**Razón**: Se usan `CustomValidationException`, `KeyNotFoundException`, y `ArgumentException` — todas estándar de .NET. No se introdujo un `Result<T>` pattern porque:
- Aumentaría la superficie de la API
- Las excepciones ya están manejadas por el middleware de ASP.NET Core
- El código es más simple y directo

### Decisión 6: Mantener Repository Pattern y Unit of Work

**Razón**: Se mantuvieron `IDentalOfficeRepository` e `IUnitOfWork` porque:
- Son abstracciones de infraestructura que el Use Case no debe conocer
- Permiten cambiar la implementación de persistencia sin tocar los Use Cases
- Son patrones establecidos de Clean Architecture

---

## Resumen Ejecutivo

| Métrica | Valor |
|---------|-------|
| Archivos eliminados | 10 |
| Archivos creados | 9 (se eliminó GetDentalOfficeDetailInput) |
| Archivos modificados | 2 |
| Tests | 7/7 pasando |
| Build | ✅ 0 errores |
| Complejidad | Reducida (sin MediatR) |
| Acoplamiento | Mínimo (DIP + ISP) |
| Testabilidad | Máxima |

**Decisión de diseño clave**: El Use Case de Get recibe `Guid id` directamente sin wrapper Input DTO, porque el parámetro viene de la ruta URL — no requiere abstracción artificial.

### Conclusión

La refactorización de CQRS a Use Cases logró:
1. **Eliminar** todas las dependencias de MediatR (`IMediator`, `IRequest`, `IRequestHandler`)
2. **Mantener** toda la funcionalidad original — equivalencia funcional 100%
3. **Reducir** la complejidad del sistema — sin framework de mediador
4. **Aumentar** la testabilidad — 7 tests unitarios cubriendo los Use Cases
5. **Mejorar** la visibilidad del intent — cada clase hace exactamente lo que su nombre indica

El sistema ahora sigue Clean Architecture de forma más pura: los Use Cases son los interactores que contienen la lógica de negocio, el Controller solo orquesta, y la infraestructura (Repository, Unit of Work) está correctamente aislada.