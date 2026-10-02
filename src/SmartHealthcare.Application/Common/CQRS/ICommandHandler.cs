using MediatR;

namespace SmartHealthcare.Application.Common.CQRS;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit> 
    where TCommand : ICommand
{
}
