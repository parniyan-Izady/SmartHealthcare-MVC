using MediatR;

namespace SmartHealthcare.Application.Common.CQRS;

/// <summary>
/// Represents a CQRS Command with a return value, implementing MediatR's IRequest.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Represents a CQRS Command without a return value.
/// </summary>
public interface ICommand : IRequest<Unit>
{
}
