namespace DataPipelines.Models;

public record SkuInformation
{
    public required string SkuId { get; init; }
    public required string ProductId { get; set; }
    public string Name { get; init; } = string.Empty;
    public Dictionary<string, string[]> StringAttributes { get; set; } = new();
    public Dictionary<string, double[]> NumberAttributes { get; set; } = new();
    public Dictionary<string, (double Min, double Max)> IntervalAttributes { get; set; } = new();
}