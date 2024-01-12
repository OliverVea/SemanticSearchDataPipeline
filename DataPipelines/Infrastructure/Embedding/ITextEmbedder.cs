using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Embedding;

public interface ITextEmbedder
{
    string Name { get; }
    Task<EmbeddingData> GetEmbeddingAsync(TextData text, CancellationToken cancellationToken);
    Task<IEnumerable<EmbeddingData>> GetEmbeddingsAsync(IEnumerable<TextData> texts, CancellationToken cancellationToken);
}