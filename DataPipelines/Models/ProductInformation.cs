namespace DataPipelines.Models;

public record ProductInformation
{
    public required string ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public string[] AlternativeSearchWords { get; set; } = Array.Empty<string>();
    public CategoryInformation[] Categories { get; set; } = Array.Empty<CategoryInformation>();
    
}