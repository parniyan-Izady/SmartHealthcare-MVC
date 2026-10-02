using MediatR;

namespace SmartHealthcare.Application.Common.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand
{
}

public interface ICommand : IRequest<Unit>, IBaseCommand
{
}
