using System.Text;
using DataPipelines.Infrastructure.Caching;
using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Embedding;

public class TextEmbeddingStore(IHashProvider hashProvider)
{
    private const int BatchSize = 256;
    private const string FileFolder = "Data";
    private const string FileExtension = ".bin";
    
    public async Task<IReadOnlyCollection<EmbeddingData>> GetEmbeddingsAsync(TextData[] texts, CancellationToken cancellationToken)
    {
        var batches = texts.Chunk(BatchSize).ToArray();
        List<EmbeddingData> embeddings = [];
        
        foreach (var batch in batches)
        {
            var tasks = batch.Select(x => GetEmbeddingAsync(x, cancellationToken));
            var batchEmbeddings = await Task.WhenAll(tasks);
            
            var validEmbeddings = (IEnumerable<EmbeddingData>)batchEmbeddings.Where(x => x is not null);
            
            embeddings.AddRange(validEmbeddings);
        }
        
        return embeddings;
    }

    private async Task<EmbeddingData?> GetEmbeddingAsync(TextData text, CancellationToken cancellationToken)
    {
        var fileName = await GetFileNameAsync(text, cancellationToken);
        if (!File.Exists(fileName)) return null;
        
        var embeddingBytes = await File.ReadAllBytesAsync(fileName, cancellationToken);
        var embedding = new float[embeddingBytes.Length / sizeof(float)];
        
        Buffer.BlockCopy(embeddingBytes, 0, embedding, 0, embeddingBytes.Length);
        
        return new EmbeddingData
        {
            SkuId = text.SkuId,
            Embedding = embedding
        };
    }

    public async Task SaveEmbeddingsAsync(IEnumerable<(TextData text, EmbeddingData embedding)> textEmbeddingPairs, CancellationToken cancellationToken)
    {
        var batches = textEmbeddingPairs.Chunk(BatchSize).ToArray();
        
        foreach (var batch in batches)
        {
            var tasks = batch.Select(x => SaveEmbeddingAsync(x, cancellationToken));
            await Task.WhenAll(tasks);
        }
    }

    private async Task SaveEmbeddingAsync((TextData text, EmbeddingData embedding) textEmbeddingPair, CancellationToken cancellationToken)
    {
        var (text, embedding) = textEmbeddingPair;
        var fileName = await GetFileNameAsync(text, cancellationToken);
        
        var embeddingBytes = new byte[embedding.Embedding.Length * sizeof(float)];
        Buffer.BlockCopy(embedding.Embedding, 0, embeddingBytes, 0, embeddingBytes.Length);
        
        await File.WriteAllBytesAsync(fileName, embeddingBytes, cancellationToken);
    }

    private async Task<string> GetFileNameAsync(TextData text, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();

        sb.Append(FileFolder + "/");
        sb.Append(nameof(TextEmbeddingStore));
        var textHash = await hashProvider.GetHashAsync(text.Text, cancellationToken);
        textHash = textHash.Replace("/", "_");
        sb.Append(textHash);
        sb.Append(FileExtension);

        return sb.ToString();
    }
}