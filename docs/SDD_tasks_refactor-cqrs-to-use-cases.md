# Tasks — Refactor Application Layer from CQRS to Use Cases

## Change: `cqrs-to-use-cases`

---

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~350-400 |
| 400-line budget risk | **Low** |
| Chained PRs recommended | **No** |
| Suggested split | Single PR (3 logical slices) |
| Decision needed before apply | **No** |

---

## Implementation Order

1. **Phase 1–3** (DTOs → Interfaces → Implementations): builds the new Use Cases layer bottom-up
2. **Phase 4** (Controller): wires the new layer into the API surface
3. **Phase 5** (DI): makes the whole thing injectable
4. **Phase 6** (Cleanup): removes the old CQRS artifacts once everything is verified working

---

## Phase 1: DTOs (Foundation — no dependencies)

- [x] 1.1 Create `Application/UseCases/DentalOffices/CreateDentalOffice/Dto/CreateDentalOfficeInput.cs`
- [x] 1.2 Create `Application/UseCases/DentalOffices/CreateDentalOffice/Dto/CreateDentalOfficeOutput.cs`
- [x] 1.3 Create `Application/UseCases/DentalOffices/GetDentalOfficeDetail/Dto/GetDentalOfficeDetailOutput.cs`
- [x] 1.4 ~~Create `Application/UseCases/DentalOffices/GetDentalOfficeDetail/GetDentalOfficeDetailInput.cs`~~ — **Eliminado**: Get no necesita Input DTO, el Guid viene directo de la ruta

---

## Phase 2: Use Case Interfaces

- [x] 2.1 Create `Application/UseCases/DentalOffices/CreateDentalOffice/ICreateDentalOfficeUseCase.cs`
- [x] 2.2 Create `Application/UseCases/DentalOffices/GetDentalOfficeDetail/IGetDentalOfficeDetailUseCase.cs`

---

## Phase 3: Use Case Implementations

- [x] 3.1 Create `Application/UseCases/DentalOffices/CreateDentalOffice/CreateDentalOfficeUseCase.cs`
- [x] 3.2 Create `Application/UseCases/DentalOffices/GetDentalOfficeDetail/GetDentalOfficeDetailUseCase.cs`

---

## Phase 4: Controller Refactor

- [x] 4.1 Modify `Controllers/DentalOfficeController.cs`

---

## Phase 5: DI Registration

- [x] 5.1 Modify `Application/RegisterApplicationServices.cs`

---

## Phase 6: Cleanup

- [x] 6.1 Delete `Application/Utilities/IRequestHandler.cs`
- [x] 6.2 Delete `Application/Utilities/IRequest.cs`
- [x] 6.3 Delete `Application/Utilities/SimpleMediator.cs`
- [x] 6.4 Delete `Application/Utilities/IMediator.cs`
- [x] 6.5 Delete `Application/Utilities/` directory
- [x] 6.6 Delete `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommand.cs` — **NOTE**: recreate without MediatR dependency since validator still needs it
- [x] 6.7 Delete `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommandHandler.cs`
- [x] 6.8 Delete `Application/Features/DentalOffices/Commands/CreateDentalOffice/CreateDentalOfficeCommandValidator.cs` — **NOTE**: validator still needed by use case, kept in Features
- [x] 6.9 Delete `Application/Features/DentalOffices/Commands/` directory
- [x] 6.10 Delete `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/GetDentalOfficeDetailQuery.cs`
- [x] 6.11 Delete `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/GetDentalOfficeDetailQueryHandler.cs`
- [x] 6.12 Delete `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/MapperExtensions.cs`
- [x] 6.13 Delete `Application/Features/DentalOffices/Queries/GetDentalOfficeDetail/DentalOfficeDetailDto.cs`
- [x] 6.14 Delete `Application/Features/DentalOffices/Queries/` directory
- [x] 6.15 Delete `Application/Features/DentalOffices/` directory
- [x] 6.16 Delete `Application/Features/` directory

### Cleanup Verification

- [x] `dotnet build` succeeds
- [x] `dotnet test` passes (if tests exist)
- [x] Manual smoke test: POST `/api/dentaloffice` creates office, GET `/api/dentaloffice/{id}` retrieves it

---

## Suggested Work Units (Commits)

| Unit | Tasks | Goal |
|------|-------|------|
| **Unit 1** | Phase 1–3 | DTOs, interfaces, implementations — core new code |
| **Unit 2** | Phase 4–5 | Controller refactor + DI registration — integration point |
| **Unit 3** | Phase 6 | Cleanup — delete 13 files |

---

## Task Summary

| Phase | Tasks | Description |
|-------|-------|-------------|
| 1 | 4 | DTOs |
| 2 | 2 | Interfaces |
| 3 | 2 | Implementations |
| 4 | 1 | Controller refactor |
| 5 | 1 | DI registration |
| 6 | 16 | Cleanup (13 file deletions + directory cleanup) |
| **Total** | **26** | |