//using Mediator_REST_API.Application.Exceptions;
//using FluentValidation;
//using FluentValidation.Results;
//using Microsoft.Extensions.DependencyInjection;

//namespace Mediator_REST_API.Application.Utilities;

//public class SimpleMediator : IMediator
//{
//    private readonly IServiceProvider _serviceProvider;

//    public SimpleMediator(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//    }

//    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
//    {
//        ArgumentNullException.ThrowIfNull(request);

//        var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());
//        var validator = _serviceProvider.GetService(validatorType);
//        if (validator is not null)
//        {
//            var validateMethod = validatorType.GetMethod("ValidateAsync");
//            var taskToValidate = (Task)validateMethod!.Invoke(validator, new object[] { request, CancellationToken.None })!;
//            await taskToValidate;

//            var result = taskToValidate.GetType().GetProperty("Result");
//            var validationResult = (ValidationResult)result!.GetValue(taskToValidate)!;
//            if (!validationResult.IsValid)
//            {
//                throw new CustomValidationException(validationResult);
//            }
//        }

//        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
//        var handler = _serviceProvider.GetService(handlerType);
//        if (handler is null)
//        {
//            throw new MediatorException($"Handler no registrado para {request.GetType().Name}");
//        }

//        var method = handlerType.GetMethod("Handle")!;
//        return await (Task<TResponse>)method.Invoke(handler, new object[] { request })!;
//    }
//}
