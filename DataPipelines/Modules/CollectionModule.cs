using DataPipelines.Core;

namespace DataPipelines.Modules;

public class CollectionModule<T> : IDataPipelineModule
{
    private const int BatchSize = 8192;
    
    public DataPipe<T>? InputPipe { get; set; }
    public string Name => nameof(CollectionModule<T>);
    public bool Finished { get; private set; }
    public Action OnFinished { get; set; } = () => { };
    
    
    private readonly List<T> _collection = [];
    public IReadOnlyCollection<T> Collection => _collection;
    
    public Task ProcessAsync(CancellationToken cancellationToken)
    {
        Process();
        return Task.CompletedTask;
    }

    private void Process()
    {
        if (Finished || InputPipe is not { } inputPipe) return;
        
        var input = inputPipe.Dequeue(BatchSize);
        _collection.AddRange(input);

        if (inputPipe is { FinishedInput: true, IsEmpty: true })
        {
            Finished = true;
            OnFinished();
        }
    }
}