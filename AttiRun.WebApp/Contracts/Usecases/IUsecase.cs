namespace AttiRun.WebApp.Contracts.Usecases;

public interface IUsecase<T>
{
    public Task<T> HandleAsync(CancellationToken cancellationToken);
}
