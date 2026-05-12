namespace Mediator_REST_API.Application.Exceptions;

public class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public string KeyName { get; }

    public EntityNotFoundException(string entityName, object key)
        : base($"{entityName} with Id {key} was not found.")
    {
        EntityName = entityName;
        KeyName = key.ToString() ?? string.Empty;
    }
}