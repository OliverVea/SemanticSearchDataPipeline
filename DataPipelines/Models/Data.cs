namespace DataPipelines.Models;

public abstract record Data
{
    public required string SkuId { get; init; }
}