using MediatR;

namespace InfoTrack_SEOWeb.Handler.Infrastructure;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}

public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
{
}
