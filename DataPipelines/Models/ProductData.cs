namespace DataPipelines.Models;

public record ProductData : Data
{
    public required SkuInformation Sku { get; set; }
    public required ProductInformation Product { get; set; }
}