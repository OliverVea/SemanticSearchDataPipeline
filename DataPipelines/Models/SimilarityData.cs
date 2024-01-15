namespace DataPipelines.Models;

public record SimilarityData : Data
{
    public double Similarity { get; init; }
}