namespace DataPipelines.Infrastructure.Loading;

public class SkuDocumentModel
{
    public string SegmentId { get; set; } = string.Empty;
    public SkuModel[] Skus { get; set; } = Array.Empty<SkuModel>();
    
    public class SkuModel
    {
        public string SkuId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, string[]> StringAttributes { get; set; } = new();
        public Dictionary<string, double[]> NumberAttributes { get; set; } = new();
        public Dictionary<string, (double Min, double Max)> IntervalAttributes { get; set; } = new();
    }
}
