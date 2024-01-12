using System.Security.Cryptography;
using System.Text;
using DataPipelines.Core;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class DistinctProductModule(ILogger<DistinctProductModule> logger) : DataPipelineModule<ProductData, ProductData>(logger)
{
    private readonly HashSet<string> _seenProductIds = [];

    public override string Name => nameof(DistinctProductModule);

    protected override Task<IReadOnlyCollection<ProductData>> ProcessAsync(IReadOnlyCollection<ProductData> inputBatch, CancellationToken cancellationToken)
    {
        var output = inputBatch.Where(x => _seenProductIds.Add(x.Product.ProductId)).ToArray();
        return Task.FromResult(output as IReadOnlyCollection<ProductData>);
    }
}