
namespace RefApi.Common;

public interface IRequestHandler<in TRequest, TResult>
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface IStreamRequestHandler<in TRequest, out TResult>
{
    IAsyncEnumerable<TResult> HandleStreamAsync(TRequest request, CancellationToken cancellationToken = default);
}
