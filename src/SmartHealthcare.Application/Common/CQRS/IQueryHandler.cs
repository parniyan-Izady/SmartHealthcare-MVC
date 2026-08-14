using MediatR;

namespace SmartHealthcare.Application.Common.CQRS;

/// <summary>
/// Defines a MediatR request handler for a CQRS Query.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse> 
    where TQuery : IQuery<TResponse>
{
}
