using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Embedding;

public class TextEmbeddingRepository(ITextEmbeddingClient client, TextEmbeddingCache cache, TextEmbeddingStore store)
{
    public async Task<IEnumerable<EmbeddingData>> GetEmbeddingsAsync(IEnumerable<TextData> texts, CancellationToken cancellationToken)
    {
        var textBySkuId = texts.ToDictionary(x => x.SkuId, x => x);
        
        var embeddings = await cache.GetEmbeddingsAsync(textBySkuId.Values, cancellationToken);
        var cachedEmbeddingSkuIds = embeddings.Select(x => x.SkuId).ToHashSet();
        
        var missingTexts = textBySkuId.Values.Where(t => !cachedEmbeddingSkuIds.Contains(t.SkuId)).ToArray();

        if (missingTexts.Any())
        {
            var missingEmbeddings = await store.GetEmbeddingsAsync(missingTexts, cancellationToken);
            embeddings = embeddings.Concat(missingEmbeddings).ToArray();
            
            var textEmbeddingPairs = missingEmbeddings.Select(x => (textBySkuId[x.SkuId], x));
            
            await cache.SaveEmbeddingsAsync(textEmbeddingPairs, cancellationToken);
        }

        var cachedAndStoredEmbeddingSkuIds = embeddings.Select(x => x.SkuId).ToHashSet();

        missingTexts = textBySkuId.Values.Where(t => !cachedAndStoredEmbeddingSkuIds.Contains(t.SkuId)).ToArray();

        if (missingTexts.Any())
        {
            var missingRawEmbeddings = await client.GetEmbeddingsAsync(missingTexts.Select(x => x.Text), cancellationToken);
            var missingEmbeddings = missingRawEmbeddings.Zip(missingTexts).Select(x => new EmbeddingData {Embedding = x.First, SkuId = x.Second.SkuId}).ToArray();
            embeddings = embeddings.Concat(missingEmbeddings).ToArray();
            
            var textEmbeddingPairs = missingEmbeddings.Select(x => (textBySkuId[x.SkuId], x)).ToArray();

            await store.SaveEmbeddingsAsync(textEmbeddingPairs, cancellationToken);
            await cache.SaveEmbeddingsAsync(textEmbeddingPairs, cancellationToken);
        }
        
        var embeddingBySkuId = embeddings.ToDictionary(x => x.SkuId, x => x);
        
        embeddings = texts.Select(x => embeddingBySkuId[x.SkuId]).ToArray();

        return embeddings;
    }
}