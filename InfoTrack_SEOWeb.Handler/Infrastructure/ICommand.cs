using MediatR;

namespace InfoTrack_SEOWeb.Handler.Infrastructure;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}

public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
{
}
