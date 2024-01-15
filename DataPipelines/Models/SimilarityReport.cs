namespace DataPipelines.Models;

public class SimilarityReport
{
    public string Query { get; set; }
    public IOrderedEnumerable<SimilarityData> Similarities { get; set; }
}