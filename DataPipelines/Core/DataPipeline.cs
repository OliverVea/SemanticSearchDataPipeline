using Microsoft.Extensions.DependencyInjection;

namespace DataPipelines.Core;

public class DataPipeline(IServiceProvider serviceProvider)
{
    private readonly List<IDataPipelineModule> _modules = [];
    
    public T AddModule<T>() where T : IDataPipelineModule
    {
        var module = ActivatorUtilities.CreateInstance<T>(serviceProvider);
        
        _modules.Add(module);
        
        return module;
    }


    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var processors = _modules.Select(m =>
        {
            var processor = ActivatorUtilities.CreateInstance<PipelineModuleProcessor>(serviceProvider);
            processor.Module = m;
            return processor;
        });
        
        await Task.WhenAll(processors.Select(p => p.RunAsync(cancellationToken)));
    }
}