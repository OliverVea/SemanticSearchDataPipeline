using DataPipelines.Models;

namespace DataPipelines.Infrastructure;

public interface ITextTransformer
{
    TextData Transform(ProductData productData, HashSet<string> allowedAttributes);
}