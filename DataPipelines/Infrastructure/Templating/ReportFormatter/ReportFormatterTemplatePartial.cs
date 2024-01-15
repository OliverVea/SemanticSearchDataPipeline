using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Templating.ReportFormatter;

public partial class ReportFormatterTemplate
{
    public required SimilarityReport Report { get; init; }
    public required Dictionary<string, ProductData> ProductData { get; init; }

    private string GetSimilarityLine(int i, SimilarityData similarity)
    {
        var productData = ProductData[similarity.SkuId];
        return $"{i + 1}. {productData.Sku.Name} ({similarity.Similarity})";
    }
}