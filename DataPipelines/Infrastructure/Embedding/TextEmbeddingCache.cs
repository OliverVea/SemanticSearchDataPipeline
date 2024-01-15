using System.Text;
using DataPipelines.Infrastructure.Caching;
using DataPipelines.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DataPipelines.Infrastructure.Embedding;

public class TextEmbeddingCache(IMemoryCache memoryCache, IHashProvider hashProvider)
{
    public async Task<IReadOnlyCollection<EmbeddingData>> GetEmbeddingsAsync(IEnumerable<TextData> texts, CancellationToken cancellationToken)
    {
        List<EmbeddingData> embeddings = [];

        foreach (var text in texts)
        {
            var cacheKey = await GetCacheKeyAsync(text, cancellationToken);
            var cacheValue = memoryCache.Get<EmbeddingData>(cacheKey);
            if (cacheValue is not null) embeddings.Add(cacheValue);
        }

        return embeddings;
    }

    public async Task SaveEmbeddingsAsync(IEnumerable<(TextData text, EmbeddingData embedding)> embeddings, CancellationToken cancellationToken)
    {
        foreach (var (text, embedding) in embeddings)
        {
            var cacheKey = await GetCacheKeyAsync(text, cancellationToken);
            memoryCache.Set(cacheKey, embedding);
        }
    }

    private async Task<string> GetCacheKeyAsync(TextData text, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();

        sb.Append(nameof(TextEmbeddingCache));
        var hashedText = await hashProvider.GetHashAsync<string>(text.Text, cancellationToken);
        sb.Append(hashedText);

        return sb.ToString();
    }
}