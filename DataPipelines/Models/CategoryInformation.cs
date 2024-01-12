namespace DataPipelines.Models;

public record CategoryInformation
{
    public CategoryPathElementInformation[] CategoryPathElements { get; set; } = Array.Empty<CategoryPathElementInformation>();
        
}