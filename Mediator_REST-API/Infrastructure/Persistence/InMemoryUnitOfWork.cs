using Mediator_REST_API.Application.Contracts.Persistence;

namespace Mediator_REST_API.Infrastructure.Persistence;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task Commit() => Task.CompletedTask;

    public Task Rollback() => Task.CompletedTask;
}
