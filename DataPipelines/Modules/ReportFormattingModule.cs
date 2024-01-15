using DataPipelines.Core;
using DataPipelines.Infrastructure.Templating.ReportFormatter;
using DataPipelines.Models;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class ReportFormattingModule(ILogger<ReportFormattingModule> logger) : DataPipelineModule<SimilarityReport, string>(logger)
{
    public override string Name => nameof(ReportFormattingModule);
    public Dictionary<string, ProductData>? ProductData { get; set; } = new();
    
    protected override Task<IReadOnlyCollection<string>> ProcessAsync(IReadOnlyCollection<SimilarityReport> inputBatch, CancellationToken cancellationToken)
    {
        if (ProductData is not { } productData) throw new InvalidOperationException("ProductData is null.");
        var output = inputBatch.Select(report => FormatReport(report, productData));
        return Task.FromResult<IReadOnlyCollection<string>>(output.ToList());
    }

    private string FormatReport(SimilarityReport report, Dictionary<string, ProductData> productData)
    {
        var template = new ReportFormatterTemplate
        {
            Report = report,
            ProductData = productData
        };

        return template.TransformText();
    }
}