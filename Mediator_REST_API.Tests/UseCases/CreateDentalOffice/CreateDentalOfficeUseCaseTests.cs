using Mediator_REST_API.Application.Contracts.Persistence;
using Mediator_REST_API.Application.Contracts.Repositories;
using Mediator_REST_API.Application.Exceptions;
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
            new CreateDentalOfficeValidator());
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsOutputWithIdAndName()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .Returns(Task.CompletedTask);

        var result = await _useCase.ExecuteAsync(input);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Dental test", result.Name);
        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyName_ThrowsCustomValidationException()
    {
        var input = new CreateDentalOfficeInput { Name = "" };

        await Assert.ThrowsAsync<CustomValidationException>(
            () => _useCase.ExecuteAsync(input));
    }

    [Fact]
    public async Task ExecuteAsync_WithWhitespaceName_ThrowsCustomValidationException()
    {
        var input = new CreateDentalOfficeInput { Name = "   " };

        await Assert.ThrowsAsync<CustomValidationException>(
            () => _useCase.ExecuteAsync(input));
    }

    [Fact]
    public async Task ExecuteAsync_OnRepositoryException_CallsRollback()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };
        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ThrowsAsync(new Exception("DB error"));
        _unitOfWorkMock.Setup(u => u.Rollback())
            .Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<Exception>(
            () => _useCase.ExecuteAsync(input));

        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_CallsCommitOnce()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .Returns(Task.CompletedTask);

        await _useCase.ExecuteAsync(input);

        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_DoesNotCallRollback()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .Returns(Task.CompletedTask);

        await _useCase.ExecuteAsync(input);

        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_CallsAddWithNewDentalOffice()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };
        DentalOffice? capturedDentalOffice = null;

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .Callback<DentalOffice>(d => capturedDentalOffice = d)
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .Returns(Task.CompletedTask);

        await _useCase.ExecuteAsync(input);

        Assert.NotNull(capturedDentalOffice);
        Assert.Equal("Dental test", capturedDentalOffice!.Name);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyName_DoesNotCallAdd()
    {
        var input = new CreateDentalOfficeInput { Name = "" };

        await Assert.ThrowsAsync<CustomValidationException>(
            () => _useCase.ExecuteAsync(input));

        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyName_DoesNotCallCommit()
    {
        var input = new CreateDentalOfficeInput { Name = "" };

        await Assert.ThrowsAsync<CustomValidationException>(
            () => _useCase.ExecuteAsync(input));

        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithWhitespaceName_DoesNotCallAdd()
    {
        var input = new CreateDentalOfficeInput { Name = "   " };

        await Assert.ThrowsAsync<CustomValidationException>(
            () => _useCase.ExecuteAsync(input));

        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithWhitespaceName_DoesNotCallCommit()
    {
        var input = new CreateDentalOfficeInput { Name = "   " };

        await Assert.ThrowsAsync<CustomValidationException>(
            () => _useCase.ExecuteAsync(input));

        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullName_ThrowsCustomValidationException()
    {
        var input = new CreateDentalOfficeInput { Name = null! };

        await Assert.ThrowsAsync<CustomValidationException>(
            () => _useCase.ExecuteAsync(input));

        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommitFails_CallsRollback()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .ThrowsAsync(new Exception("Commit failed"));
        _unitOfWorkMock.Setup(u => u.Rollback())
            .Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<Exception>(
            () => _useCase.ExecuteAsync(input));

        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommitFails_ReThrowsException()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };
        var expectedException = new Exception("Commit failed");

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .ThrowsAsync(expectedException);

        var actualException = await Assert.ThrowsAsync<Exception>(
            () => _useCase.ExecuteAsync(input));

        Assert.Equal("Commit failed", actualException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommitFails_HasCalledAddBeforeFailure()
    {
        var input = new CreateDentalOfficeInput { Name = "Dental test" };

        _repositoryMock.Setup(r => r.Add(It.IsAny<DentalOffice>()))
            .ReturnsAsync((DentalOffice d) => d);
        _unitOfWorkMock.Setup(u => u.Commit())
            .ThrowsAsync(new Exception("Commit failed"));
        _unitOfWorkMock.Setup(u => u.Rollback())
            .Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<Exception>(
            () => _useCase.ExecuteAsync(input));

        _repositoryMock.Verify(r => r.Add(It.IsAny<DentalOffice>()), Times.Once);
    }
}
