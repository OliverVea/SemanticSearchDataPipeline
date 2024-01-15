namespace DataPipelines.Models;

public record QueryEmbedding
{
    public required string Query { get; init; }
    public required float[] Embedding { get; init; }
}