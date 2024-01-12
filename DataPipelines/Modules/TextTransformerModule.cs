using DataPipelines.Core;
using DataPipelines.Infrastructure;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class TextTransformerModule<T>(
    ILogger logger,
    T transformer) : DataPipelineModule<ProductData, TextData>(logger) where T : ITextTransformer
{
    public override string Name => nameof(TextTransformerModule<T>);

    protected override Task<IReadOnlyCollection<TextData>> ProcessAsync(IReadOnlyCollection<ProductData> inputBatch, CancellationToken cancellationToken)
    {
        var output = inputBatch.Select(Process).ToArray();
        return Task.FromResult(output as IReadOnlyCollection<TextData>);
    }

    private TextData Process(ProductData productData)
    {
        return transformer.Transform(productData, []);
    }
}