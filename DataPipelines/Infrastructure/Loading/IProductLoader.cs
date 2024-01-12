using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Loading;

public interface IProductLoader
{
    Task<IEnumerable<ProductData>> LoadAsync(FileInfo productFile, FileInfo skuFile, CancellationToken cancellationToken);
}