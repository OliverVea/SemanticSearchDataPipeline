using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Infrastructure.Embedding;

public class OpenAIEmbeddingClient(
    OpenAIClient client,
    OpenAiConfiguration configuration,
    ILogger<OpenAIEmbeddingClient> logger) : ITextEmbeddingClient
{
    private const int BatchSize = 128;
    
    public async Task<IReadOnlyCollection<float[]>> GetEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken)
    {
        var chunks = texts.Chunk(BatchSize).ToArray();
        var embeddings = new List<float[]>();

        for (var i = 0; i < chunks.Length; i++)
        {
            var chunk = chunks[i];
            logger.LogInformation($"Embedding chunk {i} of {chunks.Length} texts");
            
            var embeddingsOptions = new EmbeddingsOptions(configuration.Deployment, chunk.ToArray());
        
            var response = await client.GetEmbeddingsAsync(embeddingsOptions, cancellationToken);
            if (!response.HasValue) throw new Exception("OpenAI embedding response was null");

            embeddings.AddRange(response.Value.Data.Select(x => x.Embedding.ToArray()));
        }

        return embeddings;
    }
}