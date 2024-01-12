using DataPipelines.Core;
using DataPipelines.Infrastructure;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public abstract class ProductCleaningModule<T>(
    ILogger logger,
    T cleaner) : DataPipelineModule<ProductData, ProductData>(logger) where T : IStringCleaner
{
    protected override Task<IReadOnlyCollection<ProductData>> ProcessAsync(
        IReadOnlyCollection<ProductData> inputBatch,
        CancellationToken cancellationToken)
    {
        var output = inputBatch.Select(Process).ToArray();
        return Task.FromResult(output as IReadOnlyCollection<ProductData>);
    }

    private ProductData Process(ProductData original)
    {
        return original with
        {
            Product = original.Product with
            {
                Name = cleaner.Clean(original.Product.Name),
                ShortDescription = cleaner.Clean(original.Product.ShortDescription),
                LongDescription = cleaner.Clean(original.Product.LongDescription),
                Categories = Clean(original.Product.Categories)
            },
            Sku = original.Sku with
            {
                Name = cleaner.Clean(original.Sku.Name),
                StringAttributes = cleaner.Clean(original.Sku.StringAttributes),
                NumberAttributes = cleaner.Clean(original.Sku.NumberAttributes),
                IntervalAttributes = cleaner.Clean(original.Sku.IntervalAttributes),
            }
        };
    }
    
    private CategoryInformation[] Clean(IEnumerable<CategoryInformation> original)
        => original.Select(Clean).ToArray();

    private CategoryInformation Clean(CategoryInformation original)
    {
        return original with
        {
            CategoryPathElements = Clean(original.CategoryPathElements)
        };
    }
    
    private CategoryPathElementInformation[] Clean(IEnumerable<CategoryPathElementInformation> original)
        => original.Select(Clean).ToArray();
    
    private CategoryPathElementInformation Clean(CategoryPathElementInformation original)
    {
        return original with
        {
            CategoryName = cleaner.Clean(original.CategoryName)
        };
    }
}