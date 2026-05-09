using Mediator_REST_API.Application.Contracts.Persistence;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice;
using Mediator_REST_API.Application.UseCases.DentalOffices.CreateDentalOffice.Dto;
using Mediator_REST_API.Domain.Entities;
using Moq;

namespace Mediator_REST_API.Tests.UseCases.CreateDentalOffice;

public class CreateDentalOfficeUseCaseTests
{
    private readonly Mock<IDentalOfficeRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateDentalOfficeUseCase _useCase;

    public CreateDentalOfficeUseCaseTests()
    {
        _repositoryMock = new Mock<IDentalOfficeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new CreateDentalOfficeUseCase(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            new Application.UseCases.DentalOffices.CreateDentalOffice.CreateDentalOfficeValidator());
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsOutputWithIdAndName()
    {
        // Arrange
        var input = new CreateDentalOfficeInput { Name = "Dental test" };

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Dental test", result.Name);
        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyName_ThrowsCustomValidationException()
    {
        // Arrange
        var input = new CreateDentalOfficeInput { Name = "" };

        // Act & Assert
        await Assert.ThrowsAsync<Mediator_REST_API.Application.Exceptions.CustomValidationException>(
            () => _useCase.ExecuteAsync(input));

        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithWhitespaceName_ThrowsCustomValidationException()
    {
        // Arrange
        var input = new CreateDentalOfficeInput { Name = "   " };

        // Act & Assert
        await Assert.ThrowsAsync<Mediator_REST_API.Application.Exceptions.CustomValidationException>(
            () => _useCase.ExecuteAsync(input));
    }

    [Fact]
    public async Task ExecuteAsync_OnRepositoryException_CallsRollback()
    {
        // Arrange
        var input = new CreateDentalOfficeInput { Name = "Dental test" };
        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ThrowsAsync(new Exception("DB error"));
        _unitOfWorkMock.Setup(u => u.Rollback())
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _useCase.ExecuteAsync(input));

        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
    }
}