using Microsoft.Extensions.Logging;

namespace DataPipelines.Core;

public class PipelineModuleProcessor(ILogger<PipelineModuleProcessor> logger)
{
    private static readonly TimeSpan SleepTime = TimeSpan.FromMilliseconds(100);
    public IDataPipelineModule? Module { get; set; }
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Module!.ProcessAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing module {ModuleName}", Module?.Name);
            }
            await Task.Delay(SleepTime, cancellationToken);
        }
    }
}