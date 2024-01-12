using DataPipelines.Core;
using DataPipelines.Infrastructure.Embedding;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public abstract class EmbedderModule<T>(ILogger logger, T textEmbedder) : DataPipelineModule<TextData, EmbeddingData>(logger) where T : ITextEmbedder
{
    protected override int InputBatchSize => 64;

    protected override async Task<IReadOnlyCollection<EmbeddingData>> ProcessAsync(
        IReadOnlyCollection<TextData> inputBatch,
        CancellationToken cancellationToken)
    {
        var embeddings = await textEmbedder.GetEmbeddingsAsync(inputBatch, cancellationToken);
        return embeddings.ToArray();
    }
}