using DataPipelines.Infrastructure.Loading;
using DataPipelines.Models;

namespace DataPipelines.Modules;

public class SkuProductJsonLoadingModule(IProductLoader productLoader) : DataLoadingModule<ProductData>
{
    public FileInfo? SkuFile { get; set; }
    public FileInfo? ProductFile { get; set; }
    
    protected override Task<IEnumerable<ProductData>> LoadDataAsync(CancellationToken cancellationToken)
    {
        if (SkuFile is not { } skuFile) return Task.FromResult(Array.Empty<ProductData>() as IEnumerable<ProductData>);
        if (ProductFile is not {} productFile) return Task.FromResult(Array.Empty<ProductData>() as IEnumerable<ProductData>);
        
        return productLoader.LoadAsync(productFile, skuFile, cancellationToken);
    }
}