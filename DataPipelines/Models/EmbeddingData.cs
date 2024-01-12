namespace DataPipelines.Models;

public record EmbeddingData : Data
{
    public required float[] Embedding { get; init; }
}