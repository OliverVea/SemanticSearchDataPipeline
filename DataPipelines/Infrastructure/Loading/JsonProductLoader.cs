using System.Text.Json;
using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Loading;

public class JsonProductLoader : IProductLoader
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task<IEnumerable<ProductData>> LoadAsync(FileInfo productFile, FileInfo skuFile, CancellationToken cancellationToken)
    {
        var products = await LoadProductsAsync(productFile, cancellationToken);
        var skus = await LoadSkusAsync(skuFile, cancellationToken);

        return Map(products, skus);
    }

    private async Task<IEnumerable<ProductInformation>> LoadProductsAsync(FileInfo productFile, CancellationToken cancellationToken)
    {
        var stream = productFile.OpenRead();
        
        var productDocumentModels = await JsonSerializer
            .DeserializeAsync<ProductDocumentModel[]>(stream, JsonSerializerOptions, cancellationToken);
        
        if (productDocumentModels is null) return Array.Empty<ProductInformation>();

        return productDocumentModels.SelectMany(x => x.Products).Select(Map);
    }

    private async Task<IEnumerable<SkuInformation>> LoadSkusAsync(FileInfo skuFile, CancellationToken cancellationToken)
    {
        var stream = skuFile.OpenRead();
        var skuDocumentModels = await JsonSerializer
            .DeserializeAsync<SkuDocumentModel[]>(stream, JsonSerializerOptions, cancellationToken);
        
        if (skuDocumentModels is null) return Array.Empty<SkuInformation>();

        return skuDocumentModels.SelectMany(x => x.Skus).Select(Map);
    }
    
    
    
    
    
    
    private static IEnumerable<ProductData> Map(IEnumerable<ProductInformation> productModel, IEnumerable<SkuInformation> skus)
    {
        var productDictionary = productModel.ToDictionary(x => x.ProductId);
        
        return skus
            .Where(x => productDictionary.ContainsKey(x.ProductId))
            .Select(x => new ProductData
            {
                SkuId = x.SkuId,
                Sku = x,
                Product = productDictionary[x.ProductId]
            });
    }
    
    private static ProductInformation Map(ProductDocumentModel.ProductModel productModel)
    {
        return new ProductInformation
        {
            ProductId = productModel.ProductId,
            Name = productModel.Name,
            ShortDescription = productModel.ShortDescription,
            LongDescription = productModel.LongDescription,
            AlternativeSearchWords = productModel.AlternativeSearchWords,
            Categories = productModel.Categories.Select(Map).ToArray()
        };
    }

    private static CategoryInformation Map(ProductDocumentModel.ProductModel.CategoryInformation categoryInformation)
    {
        return new CategoryInformation
        {
            CategoryPathElements = categoryInformation.CategoryPathElements.Select(Map).ToArray()
        };
    }

    private static CategoryPathElementInformation Map(ProductDocumentModel.ProductModel.CategoryInformation.CategoryPathElementInformation categoryPathElement, int arg2)
    {
        return new CategoryPathElementInformation
        {
            CategoryId = categoryPathElement.CategoryId,
            CategoryName = categoryPathElement.CategoryName
        };
    }
    
    private static SkuInformation Map(SkuDocumentModel.SkuModel skuModel)
    {
        return new SkuInformation
        {
            SkuId = skuModel.SkuId,
            ProductId = skuModel.ProductId,
            Name = skuModel.Name,
            StringAttributes = skuModel.StringAttributes,
            NumberAttributes = skuModel.NumberAttributes,
            IntervalAttributes = skuModel.IntervalAttributes
        };
    }
}