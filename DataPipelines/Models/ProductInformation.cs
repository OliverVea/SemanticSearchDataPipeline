namespace DataPipelines.Models;

public record ProductInformation
{
    public required string ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ShortDescription { get; init; } = string.Empty;
    public string LongDescription { get; init; } = string.Empty;
    public string[] AlternativeSearchWords { get; init; } = Array.Empty<string>();
    public CategoryInformation[] Categories { get; init; } = Array.Empty<CategoryInformation>();
    
}