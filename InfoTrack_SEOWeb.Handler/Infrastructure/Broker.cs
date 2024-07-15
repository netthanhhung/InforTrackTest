using MediatR;
namespace InfoTrack_SEOWeb.Handler.Infrastructure;

public interface IBroker
{
    Task<TResponse> CommandAsync<TResponse>(ICommand<TResponse> request, CancellationToken token = default);
    public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> request, CancellationToken token = default);
}

public class Broker : IBroker
{
    private readonly IMediator mediator;

    public Broker(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Task<TResponse> CommandAsync<TResponse>(ICommand<TResponse> request, CancellationToken token)
    {
        return this.mediator.Send(request, token);
    }

    public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> request, CancellationToken token)
    {
        return this.mediator.Send(request, token);
    }
}
