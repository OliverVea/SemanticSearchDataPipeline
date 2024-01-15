namespace DataPipelines.Models;

public record CategoryInformation
{
    public CategoryPathElementInformation[] CategoryPathElements { get; init; } = Array.Empty<CategoryPathElementInformation>();
        
}