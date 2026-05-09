# Spec — Refactor Application Layer from CQRS to Use Cases

## Change: `cqrs-to-use-cases`

---

## ADDED Requirements

### Requirement: ICreateDentalOfficeUseCase

The system SHALL provide an explicit use case for creating a dental office, exposed via `ICreateDentalOfficeUseCase` interface with method `Task<DentalOfficeDetailOutput> ExecuteAsync(CreateDentalOfficeInput input, CancellationToken ct)`.

#### Scenario: CreateDentalOffice — success
- GIVEN a valid `CreateDentalOfficeInput` with non-empty `Name`
- WHEN `ExecuteAsync` is invoked
- THEN the system MUST validate input, create `DentalOffice` entity, persist via `IDentalOfficeRepository.Add`, commit `IUnitOfWork`, and return `DentalOfficeDetailOutput` with generated `Id` and provided `Name`
- AND the returned `Id` MUST NOT be `Guid.Empty`

#### Scenario: CreateDentalOffice — validation failure
- GIVEN an empty or whitespace-only `Name`
- WHEN `ExecuteAsync` is invoked
- THEN the system MUST throw `CustomValidationException` with FluentValidation failure message "El campo Nombre es requerido"
- AND MUST NOT persist any entity

#### Scenario: CreateDentalOffice — repository exception
- GIVEN a valid `CreateDentalOfficeInput`
- WHEN `IDentalOfficeRepository.Add` throws an exception
- THEN the system MUST invoke `IUnitOfWork.Rollback` and rethrow the original exception
- AND MUST return HTTP 500 via controller exception handling

---

### Requirement: IGetDentalOfficeDetailUseCase

The system SHALL provide an explicit use case for retrieving a dental office, exposed via `IGetDentalOfficeDetailUseCase` interface with method `Task<GetDentalOfficeDetailOutput> ExecuteAsync(Guid id, CancellationToken ct)`.

**Nota**: GetDentalOfficeDetail NO usa Input DTO — el Guid viene directo de la ruta URL. ASP.NET Core binding ya convierte el string a Guid.

#### Scenario: GetDentalOfficeDetail — success
- GIVEN a `Guid id` that matches an existing `DentalOffice`
- WHEN `ExecuteAsync(id)` is invoked
- THEN the system MUST retrieve the entity via `IDentalOfficeRepository.GetById`, map to `GetDentalOfficeDetailOutput`, and return it with correct `Id` and `Name`

#### Scenario: GetDentalOfficeDetail — not found
- GIVEN a `Guid id` that does NOT match any existing `DentalOffice`
- WHEN `ExecuteAsync(id)` is invoked
- THEN the system MUST throw `KeyNotFoundException` with message "DentalOffice with Id {id} was not found."

#### Scenario: GetDentalOfficeDetail — empty Guid
- GIVEN `Guid.Empty` as id
- WHEN `ExecuteAsync(Guid.Empty)` is invoked
- THEN the system MUST throw `ArgumentException` with message "Id cannot be empty"

---

### Requirement: Controller Injection Refactor

The system SHALL replace `IMediator` dependency injection with direct `ICreateDentalOfficeUseCase` and `IGetDentalOfficeDetailUseCase` injection in `DentalOfficeController`.

#### Scenario: Controller uses use cases
- GIVEN `ICreateDentalOfficeUseCase` and `IGetDentalOfficeDetailUseCase` are registered in DI
- WHEN `DentalOfficeController` is instantiated
- THEN the constructor MUST accept and assign both use case dependencies
- AND `Create` action MUST call `CreateDentalOfficeUseCase.ExecuteAsync`
- AND `GetById` action MUST call `GetDentalOfficeDetailUseCase.ExecuteAsync`

---

### Requirement: DI Registration

The system SHALL register use case implementations in the DI container as scoped services: `ICreateDentalOfficeUseCase` → `CreateDentalOfficeUseCase`, `IGetDentalOfficeDetailUseCase` → `GetDentalOfficeDetailUseCase`.

#### Scenario: Use cases registered
- GIVEN the `AddApplicationServices` method executes
- THEN it MUST contain explicit scoped registrations for both use case interfaces
- AND FluentValidation validators for input DTOs MUST be registered

---

## REMOVED Requirements

### Requirement: MediatR Registration

(Reason: MediatR library is eliminated; use cases replace command/query handlers)

#### Scenario: MediatR removed
- GIVEN `RegisterApplicationServices` is called
- THEN it MUST NOT invoke `AddMediatR` or register `IMediator`, `SimpleMediator`, `IRequest<TResponse>`, or `IRequestHandler<TRequest, TResponse>`
- AND `MediatR` package reference MUST be removed from the project file

---

### Requirement: IMediator and SimpleMediator

(Reason: Custom mediator implementation in `Application/Utilities/` is eliminated)

The system MUST NOT contain `IMediator`, `SimpleMediator`, `IRequest<TResponse>`, or `IRequestHandler<TRequest, TResponse>` definitions.

---

### Requirement: Commands and Queries Folders

(Reason: CQRS folder structure replaced by flat `UseCases/` structure)

The system MUST NOT contain `Application/Features/DentalOffices/Commands/` or `Application/Features/DentalOffices/Queries/` directories.

---

## Coverage Summary

| Domain | Type | Count |
|--------|------|-------|
| DentalOffices Use Cases | Delta | 4 added, 0 modified, 3 removed |
| Scenarios | Total | 8 |

| Scenario Type | Covered |
|--------------|---------|
| Happy paths | ✅ Create success, GetById success |
| Edge cases | ✅ Empty name validation, not found, empty Guid, repository exception with rollback |
| Error states | ✅ CustomValidationException, KeyNotFoundException, ArgumentException |

**Nota**: El escenario "invalid Guid format" se eliminó porque ASP.NET Core model binding ya valida que el parámetro de ruta sea un Guid válido antes de llegar al Use Case. Si el formato es inválido, la request no llega al Use Case.