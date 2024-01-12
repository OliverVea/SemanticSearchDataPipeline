using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Templating.SimplifiedYml;

public class SimplifiedYmlTransformer : ITextTransformer
{
    public string Name => nameof(SimplifiedYmlTransformer);

    public TextData Transform(ProductData productData, HashSet<string> allowedAttributes)
    {
        var template = new SimplifiedYmlProductTemplate
        {
            AllowedAttributes = allowedAttributes,
            Product = productData.Product,
            Sku = productData.Sku
        };

        return new TextData
        {
            SkuId = productData.SkuId,
            Text = template.TransformText().Replace("\r\n", "\n")
        };
    }
}