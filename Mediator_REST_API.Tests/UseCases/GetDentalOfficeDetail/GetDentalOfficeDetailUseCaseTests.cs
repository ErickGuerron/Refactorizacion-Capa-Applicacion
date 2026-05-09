using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail;
using Mediator_REST_API.Application.UseCases.DentalOffices.GetDentalOfficeDetail.Dto;
using Mediator_REST_API.Domain.Entities;
using Moq;

namespace Mediator_REST_API.Tests.UseCases.GetDentalOfficeDetail;

public class GetDentalOfficeDetailUseCaseTests
{
    private readonly Mock<IDentalOfficeRepository> _repositoryMock;
    private readonly GetDentalOfficeDetailUseCase _useCase;

    public GetDentalOfficeDetailUseCaseTests()
    {
        _repositoryMock = new Mock<IDentalOfficeRepository>();
        _useCase = new GetDentalOfficeDetailUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingId_ReturnsOutputWithIdAndName()
    {
        // Arrange
        var dentalOffice = new DentalOffice("Dental test");
        var inputId = dentalOffice.Id;

        _repositoryMock.Setup(r => r.GetById(inputId))
            .ReturnsAsync(dentalOffice);

        // Act
        var result = await _useCase.ExecuteAsync(inputId);

        // Assert
        Assert.Equal(inputId, result.Id);
        Assert.Equal("Dental test", result.Name);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var inputId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetById(inputId))
            .ReturnsAsync((DentalOffice?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _useCase.ExecuteAsync(inputId));
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecuteAsync(emptyId));
    }
}