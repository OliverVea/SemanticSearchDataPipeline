using DataPipelines.Core;
using DataPipelines.Infrastructure.Embedding;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class EmbedderModule(
    ILogger<EmbedderModule> logger,
    TextEmbeddingRepository textEmbeddingRepository) : DataPipelineModule<TextData, EmbeddingData>(logger)
{
    public override string Name => nameof(EmbedderModule);
    protected override int InputBatchSize => 64;

    protected override async Task<IReadOnlyCollection<EmbeddingData>> ProcessAsync(
        IReadOnlyCollection<TextData> inputBatch,
        CancellationToken cancellationToken)
    {
        var embeddings = await textEmbeddingRepository.GetEmbeddingsAsync(inputBatch, cancellationToken);
        return embeddings.ToArray();
    }
}