namespace Mediator_REST_API.Application.Contracts.Persistence;

public interface IUnitOfWork
{
    Task Commit();

    Task Rollback();
}
