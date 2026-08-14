using MediatR;

namespace SmartHealthcare.Application.Common.CQRS;

/// <summary>
/// Defines a MediatR request handler for a CQRS Command with a return value.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Defines a MediatR request handler for a CQRS Command without a return value.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit> 
    where TCommand : ICommand
{
}
