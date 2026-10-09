namespace AttiRun.Application.Contracts.Usecases;

public interface IUsecase<T>
{
    public Task<T> HandleAsync(CancellationToken cancellationToken);
}
