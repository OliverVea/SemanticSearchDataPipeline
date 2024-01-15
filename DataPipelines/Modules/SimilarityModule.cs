using DataPipelines.Core;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class SimilarityModule<T>(ILogger<SimilarityModule<T>> logger, T similarityCalculator) : DataPipelineModule<EmbeddingData, SimilarityReport>(logger) where T : ISimilarityCalculator
{
    public override string Name => nameof(SimilarityModule<T>);
    
    private readonly PriorityQueue<SimilarityData, double> _priorityQueue = new();
    
    public QueryEmbedding? QueryEmbedding { get; set; }
    public int TopK { get; set; } = 10;

    protected override Task<IReadOnlyCollection<SimilarityReport>> ProcessAsync(
        IReadOnlyCollection<EmbeddingData> inputBatch,
        CancellationToken cancellationToken)
    {
        var result = Process(inputBatch);
        return Task.FromResult(result);
    }

    private IReadOnlyCollection<SimilarityReport> Process(IReadOnlyCollection<EmbeddingData> inputBatch)
    {
        if (QueryEmbedding is null)
        {
            throw new InvalidOperationException("QueryEmbedding is null");
        }
        
        foreach (var embeddingData in inputBatch)
        {
            var similarity = similarityCalculator.GetSimilarity(embeddingData.Embedding, QueryEmbedding.Embedding);
            _priorityQueue.Enqueue(new SimilarityData
            {
                SkuId = embeddingData.SkuId,
                Similarity = similarity
            }, similarity);
        }

        var delta = _priorityQueue.Count - TopK;
        for (var i = 0; i < delta; i++) _priorityQueue.Dequeue();

        return Array.Empty<SimilarityReport>();
    }

    protected override void OnFinished()
    {
        if (QueryEmbedding is null) return;
        
        var similarityReport = new SimilarityReport
        {
            Query = QueryEmbedding.Query,
            Similarities = _priorityQueue.UnorderedItems.Select(x => x.Element).OrderByDescending(x => x.Similarity)
        };

        foreach (var outputPipe in OutputPipes) outputPipe.Enqueue(similarityReport);
    }
}

public interface ISimilarityCalculator
{
    double GetSimilarity(float[] embeddingDataEmbedding, float[] queryEmbeddingEmbedding);
}

public class CosineSimilarityCalculator : ISimilarityCalculator
{
    public double GetSimilarity(float[] embeddingDataEmbedding, float[] queryEmbeddingEmbedding)
    {
        var dotProduct = 0.0;
        var normA = 0.0;
        var normB = 0.0;
        
        for (var i = 0; i < embeddingDataEmbedding.Length; i++)
        {
            dotProduct += embeddingDataEmbedding[i] * queryEmbeddingEmbedding[i];
            normA += Math.Pow(embeddingDataEmbedding[i], 2);
            normB += Math.Pow(queryEmbeddingEmbedding[i], 2);
        }

        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}