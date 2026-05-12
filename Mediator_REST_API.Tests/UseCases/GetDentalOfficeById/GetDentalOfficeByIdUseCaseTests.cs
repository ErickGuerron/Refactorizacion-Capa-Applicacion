using Mediator_REST_API.Application.Exceptions;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeById.Dto;
using Mediator_REST_API.Domain.Entities;
using Moq;

namespace Mediator_REST_API.Tests.UseCases.GetDentalOfficeById;

public class GetDentalOfficeByIdUseCaseTests
{
    private readonly Mock<IDentalOfficeRepository> _repositoryMock;
    private readonly GetDentalOfficeByIdUseCase _useCase;

    public GetDentalOfficeByIdUseCaseTests()
    {
        _repositoryMock = new Mock<IDentalOfficeRepository>();
        _useCase = new GetDentalOfficeByIdUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_ReturnsOutputWithIdAndName()
    {
        var dentalOffice = new DentalOffice("Dental test");
        var inputId = dentalOffice.Id;

        _repositoryMock.Setup(r => r.GetById(inputId))
            .ReturnsAsync(dentalOffice);

        var result = await _useCase.ExecuteAsync(inputId);

        Assert.Equal(inputId, result.Id);
        Assert.Equal("Dental test", result.Name);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingId_ThrowsEntityNotFoundException()
    {
        var inputId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetById(inputId))
            .ReturnsAsync((DentalOffice?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _useCase.ExecuteAsync(inputId));
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyGuid_ThrowsArgumentException()
    {
        var emptyId = Guid.Empty;

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecuteAsync(emptyId));
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_CallsRepositoryGetByIdOnce()
    {
        var dentalOffice = new DentalOffice("Dental test");
        var inputId = dentalOffice.Id;

        _repositoryMock.Setup(r => r.GetById(inputId))
            .ReturnsAsync(dentalOffice);

        await _useCase.ExecuteAsync(inputId);

        _repositoryMock.Verify(r => r.GetById(inputId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_PassesCorrectIdToRepository()
    {
        var dentalOffice = new DentalOffice("Dental test");
        var expectedId = dentalOffice.Id;

        _repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(dentalOffice);

        await _useCase.ExecuteAsync(expectedId);

        _repositoryMock.Verify(r => r.GetById(expectedId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_DoesNotCallOtherRepositoryMethods()
    {
        var dentalOffice = new DentalOffice("Dental test");
        var inputId = dentalOffice.Id;

        _repositoryMock.Setup(r => r.GetById(inputId))
            .ReturnsAsync(dentalOffice);

        await _useCase.ExecuteAsync(inputId);

        _repositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Never);
    }
}
