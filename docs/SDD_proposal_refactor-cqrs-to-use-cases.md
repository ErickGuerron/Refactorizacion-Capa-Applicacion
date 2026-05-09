# Proposal: Refactor Application Layer from CQRS to Use Cases

## Intent

Replace MediatR-based CQRS with explicit Use Case classes. The current `IMediator.Send()` indirection obscures business flow and adds a framework dependency. Use Cases make intent visible at the call site and eliminate the MediatR dependency entirely.

## Scope

### In Scope
- Create `Application/UseCases/DentalOffices/CreateDentalOffice/` with interface, implementation, dto
- Create `Application/UseCases/DentalOffices/GetDentalOfficeDetail/` with interface, implementation, dto
- Refactor `DentalOfficeController` to inject use cases directly (remove `IMediator`)
- Update `RegisterApplicationServices.cs` to wire use cases via DI
- Remove commented-out `SimpleMediator` and related utilities

### Out of Scope
- New business logic — functional equivalence required
- Infrastructure layer changes
- Repository implementation changes

## Capabilities

### New Capabilities
None — structural refactor only.

### Modified Capabilities
None — the two operations (CreateDentalOffice, GetDentalOfficeDetail) retain identical behavior.

## Approach

1. **Leer archivos actuales** — mapear estructura exacta (Commands/Queries → UseCases)
2. **Crear interfaces de Use Case** — `ICreateDentalOfficeUseCase`, `IGetDentalOfficeDetailUseCase`
3. **Crear implementaciones de Use Case** — mover lógica de los handlers, inyectar repository + unit of work
4. **Crear DTOs** en `UseCases/DentalOffices/*/Dto/` siguiendo PascalCase/camelCase
5. **Refactorizar controller** — reemplazar inyección `IMediator` por inyección directa de use cases
6. **Actualizar registro DI** en `RegisterApplicationServices.cs`
7. **Eliminar** utilidades MediatR comentadas (`SimpleMediator.cs`, `IMediator.cs`, `IRequest.cs`, `IRequestHandler.cs`)
8. **Eliminar** carpetas `Commands/` y `Queries/` con sus handlers
9. **Remover** paquete MediatR del `.csproj`

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `Controllers/DentalOfficeController.cs` | Modified | Inject use cases, remove `IMediator` |
| `Application/UseCases/` | New | Use case interfaces + implementations |
| `Application/RegisterApplicationServices.cs` | Modified | Wire use cases via DI |
| `Application/Utilities/` | Removed | `SimpleMediator`, `IMediator`, `IRequest`, `IRequestHandler` |
| `Application/Features/DentalOffices/Commands/` | Removed | `CreateDentalOfficeCommand`, `CreateDentalOfficeCommandHandler` |
| `Application/Features/DentalOffices/Queries/` | Removed | `GetDentalOfficeDetailQuery`, `GetDentalOfficeDetailQueryHandler` |

## Target Structure

```
Application/
├── UseCases/
│   └── DentalOffices/
│       ├── CreateDentalOffice/
│       │   ├── ICreateDentalOfficeUseCase.cs
│       │   ├── CreateDentalOfficeUseCase.cs
│       │   └── Dto/
│       │       ├── CreateDentalOfficeInput.cs
│       │       └── CreateDentalOfficeOutput.cs
│       └── GetDentalOfficeDetail/
│           ├── IGetDentalOfficeDetailUseCase.cs
│           ├── GetDentalOfficeDetailUseCase.cs
│           └── Dto/
│               └── GetDentalOfficeDetailOutput.cs
│           (Sin Input DTO — el Guid viene directo de la ruta)
```

## C# Naming Conventions

| Elemento | Nomenclatura | Ejemplo |
|----------|-------------|---------|
| Clases | PascalCase | `DentalOfficeService` |
| Interfaces | PascalCase con prefijo I | `IDentalOfficeRepository` |
| Métodos | PascalCase | `CreateDentalOffice()` |
| Propiedades | PascalCase | `DentalOfficeName` |
| Variables locales | camelCase | `dentalOfficeId` |
| Parámetros | camelCase | `cancellationToken` |
| Campos privados | camelCase con `_` | `_dentalOfficeRepository` |
| Constantes | PascalCase | `MaxAllowedAppointments` |
| Carpetas/namespaces | PascalCase | `Application.UseCases.DentalOffices` |
| Métodos asíncronos | PascalCase + Async | `GetByIdAsync()` |

## Why One Interface Per Use Case?

Siguiendo el **Principio de Dependencias Inversas (DIP)** de SOLID:

> Los módulos de alto nivel no deben depender de módulos de bajo nivel. Ambos deben depender de abstracciones.

**Beneficios**:
1. **Inversión de dependencias**: El Controller depende de `ICreateDentalOfficeUseCase` (abstracción), no de `CreateDentalOfficeUseCase` (implementación)
2. **Testabilidad**: Se puede mockear el Use Case en tests sin tocar el Controller
3. **Desacoplamiento**: Cambiar la implementación del Use Case no afecta al Controller
4. **Cumplimiento de SOLID**: ISP (Interface Segregation) — cada Use Case tiene su propia interfaz pequeña y enfocada
5. **Clean Architecture**: Las capas externas dependen de abstracciones de la capa de Application

## Comparison: CQRS vs Application Services vs Use Cases

| Elemento | CQRS (este proyecto) | Application Services | Use Cases |
|----------|---------------------|---------------------|-----------|
| Crear consultorio | `CreateDentalOfficeCommand` + `CreateDentalOfficeCommandHandler` | `IDentalOfficeService.CreateAsync()` | `ICreateDentalOfficeUseCase.ExecuteAsync()` |
| Consultar consultorio | `GetDentalOfficeDetailQuery` + `GetDentalOfficeDetailQueryHandler` | `IDentalOfficeService.GetByIdAsync()` | `IGetDentalOfficeDetailUseCase.ExecuteAsync()` |
| Organización de carpetas | `Commands/` y `Queries/` | `Services/` | `UseCases/` |
| Clase central | Un Handler por operación | Una clase con múltiples métodos | Una clase por caso de uso (con interfaz dedicada) |
| Mediador | Necesario (`IMediator`) | Opcional | Opcional |
| Interfaz por operación | No (Handler implementa `IRequestHandler`) | No (una interfaz para todo el servicio) | **Sí — una interfaz por Use Case** |
| Popularidad en .NET | Muy alta | Alta | Alta |
| Referencia conocida | MediatR | ASP.NET Core tradicional | Clean Architecture de Uncle Bob |

## Why Use Cases?

1. **Visibilidad de intent**: El nombre del Use Case deja claro qué hace el sistema
2. **Sin overhead de framework**: No necesita MediatR ni otro mediador
3. **Testabilidad**: Cada Use Case es una clase concreta, fácil de unittestear
4. **Acoplamiento mínimo**: El controlador conoce el Use Case, no un mediador genérico
5. **Single Responsibility**: Un Use Case = una operación de negocio
6. **ISP compliance**: Cada interfaz es pequeña y enfocada en una sola operación

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Functional regression | Low | Use cases mirror handler logic exactly |
| Validator location change | Medium | Keep validators in original namespace, reference from use cases |
| DI registration gap | Medium | Verify `Program.cs` bootstraps correctly after change |

## Rollback Plan

1. Revert `DentalOfficeController.cs` to inject `IMediator` and call `Send()`
2. Restore `RegisterApplicationServices.cs` handler registrations
3. Uncomment removed utility files
4. Delete new `UseCases/` folder

## Dependencies

- None — pure refactor, no external dependencies

## Success Criteria

- [ ] Controller usa `ICreateDentalOfficeUseCase` e `IGetDentalOfficeDetailUseCase` directamente
- [ ] No existen referencias a `IMediator`, `IRequest`, `IRequestHandler` en la capa Application
- [ ] No permanecen carpetas `Commands/` o `Queries/`
- [ ] Use Cases registrados en `RegisterApplicationServices.cs`
- [ ] Tests existentes pasan (operaciones create + get)
- [ ] FluentValidation sigue validando inputs
- [ ] Repository Pattern y Unit of Work preservados
- [ ] Todos los nombres siguen PascalCase para clases/interfaces/métodos/propiedades
- [ ] Parámetros y variables locales siguen camelCase
- [ ] Namespaces siguen PascalCase (ej: `Application.UseCases.DentalOffices.CreateDentalOffice`)
- [ ] Métodos asíncronos terminan en `Async` (ej: `ExecuteAsync`, `GetByIdAsync`)

## Constraints Respected

| Restricción | Cómo se cumple |
|-------------|----------------|
| NO IRequest, IRequestHandler, IMediator | Sustituidos por interfaces de Use Case |
| NO Commands, Queries, Handlers | Estructura de Use Cases los reemplaza |
| Mantener Repository Pattern | IDentalOfficeRepository sigue en Contracts |
| Mantener Unit of Work | IUnitOfWork inyectado en Use Cases |
| Mantener validaciones | FluentValidation preservado |
| Mantener DTOs | DTOs reubicados en dto/ de cada Use Case |
| Mantener async/await | Todos los métodos son async |
| Mantener DI | Uso Cases registrados en RegisterApplicationServices |
| Clean Architecture | Domain no se modifica, Infrastructure separado |
| Sin sobreingeniería | Estructura simple y directa |

## Technical Justification

La decisión de usar **Use Cases** sobre **Application Services** se justifica en:

1. **Cohesión**: Cada clase tiene una única responsabilidad (un Use Case = una operación)
2. **Nomenclatura explícita**: `CreateDentalOfficeUseCase` comunica intent mejor que `DentalOfficeService.CreateAsync()`
3. **Escalabilidad**: Agregar un nuevo caso de uso no requiere modificar una clase existente de servicio
4. **Testabilidad**: Mocking de un Use Case es directo, sin intermediarios
5. **Alignación con Clean Architecture**: Uncle Bob propone Use Cases como interacciones del sistema

La contra principal es más clases files → pero el tradeoff en claridad y mantenibilidad lo hace favorable para este proyecto educativo.