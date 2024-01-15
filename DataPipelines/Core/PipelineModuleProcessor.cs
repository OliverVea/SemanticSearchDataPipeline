using Microsoft.Extensions.Logging;

namespace DataPipelines.Core;

public class PipelineModuleProcessor(ILogger<PipelineModuleProcessor> logger)
{
    private static readonly TimeSpan SleepTime = TimeSpan.FromMilliseconds(100);
    public IDataPipelineModule? Module { get; set; }
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (Module is not {} module) throw new InvalidOperationException("Module is null.");
        
        while (!module.Finished && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                await module.ProcessAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing module {ModuleName}", module.Name);
            }
            await Task.Delay(SleepTime, cancellationToken);
        }
        
        logger.LogInformation("Module {ModuleName} finished.", module.Name);
    }
}