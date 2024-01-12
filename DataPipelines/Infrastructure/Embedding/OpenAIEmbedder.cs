using Azure.AI.OpenAI;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Infrastructure.Embedding;

public class OpenAIEmbedder(
    OpenAIClient client,
    OpenAiConfiguration configuration,
    ILogger<OpenAIEmbedder> logger) : ITextEmbedder
{
    public string Name => nameof(OpenAIEmbedder);

    public async Task<EmbeddingData> GetEmbeddingAsync(TextData text, CancellationToken cancellationToken)
    {
        var embeddingsOptions = new EmbeddingsOptions(
            configuration.Deployment,
            [text.Text]);
        
        var response = await client.GetEmbeddingsAsync(embeddingsOptions, cancellationToken);
        if (!response.HasValue) throw new Exception("OpenAI embedding response was null");

        return Map(text, response.Value.Data.Single());
    }

    public async Task<IEnumerable<EmbeddingData>> GetEmbeddingsAsync(IEnumerable<TextData> texts, CancellationToken cancellationToken)
    {
        var chunks = texts.Chunk(128).ToArray();
        var embeddings = new List<EmbeddingData>();

        for (var i = 0; i < chunks.Length; i++)
        {
            var chunk = chunks[i];
            logger.LogInformation($"Embedding chunk {i} of {chunks.Length} texts");
            
            var embeddingsOptions = new EmbeddingsOptions(
                    configuration.Deployment, 
                    chunk.Select(x => x.Text).ToArray());
        
            var response = await client.GetEmbeddingsAsync(embeddingsOptions, cancellationToken);
            if (!response.HasValue) throw new Exception("OpenAI embedding response was null");

            var chunkEmbeddings = chunk.Zip(response.Value.Data).Select(v => Map(v.First, v.Second));
            embeddings.AddRange(chunkEmbeddings);
        }

        return embeddings;
    }

    private static EmbeddingData Map(TextData text, EmbeddingItem embeddingItem)
    {
        return new EmbeddingData
        {
            SkuId = text.SkuId,
            Embedding = embeddingItem.Embedding.ToArray()
        };
    }
}