using MediatR;

namespace SmartHealthcare.Application.Common.CQRS;

/// <summary>
/// Represents a CQRS Query that returns data of type TResponse, implementing MediatR's IRequest.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
