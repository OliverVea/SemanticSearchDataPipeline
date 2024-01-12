namespace DataPipelines.Infrastructure.Caching;

public interface IHashProvider
{
    Task<string> GetHashAsync<T>(T input, CancellationToken cancellationToken) where T : class;
}