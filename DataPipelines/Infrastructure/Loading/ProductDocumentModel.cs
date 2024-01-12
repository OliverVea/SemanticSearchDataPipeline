namespace DataPipelines.Infrastructure.Loading;

public class ProductDocumentModel
{
    public string SegmentId { get; set; } = string.Empty;
    public ProductModel[] Products { get; set; } = Array.Empty<ProductModel>();
    
    public class ProductModel
    {
        public required string ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public string[] AlternativeSearchWords { get; set; } = Array.Empty<string>();
        public CategoryInformation[] Categories { get; set; } = Array.Empty<CategoryInformation>();
        
        public class CategoryInformation
        {
            public CategoryPathElementInformation[] CategoryPathElements { get; set; } = Array.Empty<CategoryPathElementInformation>();
            
            public class CategoryPathElementInformation
            {
                public string CategoryId { get; set; } = string.Empty;
                public string CategoryName { get; set; } = string.Empty;
            }
        }
    }
}
