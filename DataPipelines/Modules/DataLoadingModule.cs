using DataPipelines.Core;
using DataPipelines.Extensions;
using DataPipelines.Models;

namespace DataPipelines.Modules;

public abstract class DataLoadingModule<TOut> : IDataPipelineModule
{
    public string Name => nameof(DataLoadingModule<TOut>);
    
    public DataPipe<TOut>? OutputPipe
    {
        get => OutputPipes.FirstOrDefault();
        set => OutputPipes.SetFirst(value);
    }
    
    public IList<DataPipe<TOut>> OutputPipes { get; } = new List<DataPipe<TOut>>();
    public bool Finished { get; private set; }
    
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        if (!OutputPipes.Any() || Finished) return;
        
        var productData = await LoadDataAsync(cancellationToken);
        productData = productData as TOut[] ?? productData.ToArray();
        
        foreach (var outputPipe in OutputPipes) outputPipe.Enqueue(productData);
        
        Finished = true;
        foreach (var outputPipe in OutputPipes) outputPipe.FinishedInput = true;
    }

    protected abstract Task<IEnumerable<TOut>> LoadDataAsync(CancellationToken cancellationToken);
}