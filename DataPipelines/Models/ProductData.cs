namespace DataPipelines.Models;

public record ProductData : Data
{
    public required SkuInformation Sku { get; init; }
    public required ProductInformation Product { get; init; }
}