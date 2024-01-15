namespace DataPipelines.Infrastructure.Embedding;

public interface ITextEmbeddingClient
{
    Task<IReadOnlyCollection<float[]>> GetEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken);
}