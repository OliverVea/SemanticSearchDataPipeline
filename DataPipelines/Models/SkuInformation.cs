namespace DataPipelines.Models;

public record SkuInformation
{
    public required string SkuId { get; init; }
    public required string ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Dictionary<string, string[]> StringAttributes { get; init; } = new();
    public Dictionary<string, double[]> NumberAttributes { get; init; } = new();
    public Dictionary<string, (double Min, double Max)> IntervalAttributes { get; init; } = new();
}