using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using DataPipelines.Extensions;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Core;

public abstract class DataPipelineModule<TIn, TOut>(ILogger logger) : IDataPipelineModule
{
    public DataPipe<TIn>? InputPipe { get; set; }
    
    public DataPipe<TOut>? OutputPipe
    {
        get => OutputPipes.FirstOrDefault();
        set => OutputPipes.SetFirst(value);
    }

    public IList<DataPipe<TOut>> OutputPipes { get; } = new List<DataPipe<TOut>>();
    
    private ProcessingState _processingState = ProcessingState.None;
    
    public abstract string Name { get; }
    public bool Finished { get; private set; }
    protected virtual int OutputFullThreshold => 256;
    protected virtual int InputBatchSize => 256;

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        if (Finished) return;
        
        var inputPipe = InputPipe;
        if (!UpdateProcessingState(inputPipe, cancellationToken)) return;
        
        var inputBatch = inputPipe.Dequeue(InputBatchSize);
        if (inputBatch.Count == 0)
        {
            _processingState = ProcessingState.WaitingForInput;
            return;
        }
        
        logger.LogInformation($"Processing {inputBatch.Count} items in {Name}.");
        
        var stopwatch = Stopwatch.StartNew();
        var outputBatch = await ProcessAsync(inputBatch, cancellationToken);
        stopwatch.Stop();
        
        logger.LogInformation($"Processed {inputBatch.Count} items in {Name} in {stopwatch.ElapsedMilliseconds}ms.");
        
        foreach (var outputPipe in OutputPipes) outputPipe.Enqueue(outputBatch);
    }

    protected abstract Task<IReadOnlyCollection<TOut>> ProcessAsync(
        IReadOnlyCollection<TIn> inputBatch,
        CancellationToken cancellationToken);

    protected bool UpdateProcessingState(
        [NotNullWhen(true)] DataPipe<TIn>? inputPipe,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) _processingState = ProcessingState.Cancelled;
        else if (inputPipe is null) _processingState = ProcessingState.NoInputPipe;
        else if (OutputPipes.Count == 0) _processingState = ProcessingState.NoOutputPipes;
        else if (inputPipe is { FinishedInput: true, IsEmpty: true })
        {
            _processingState = ProcessingState.Finished;
            Finished = true;
            OnFinished();
            foreach (var outputPipe in OutputPipes) outputPipe.FinishedInput = true;
        }
        else if (inputPipe.IsEmpty) _processingState = ProcessingState.WaitingForInput;
        else if (OutputPipes.Any(x => x.Count >= OutputFullThreshold)) _processingState = ProcessingState.OutputQueueFull;
        else _processingState = ProcessingState.Processing;
        
        return _processingState == ProcessingState.Processing;
    }
    
    protected virtual void OnFinished()
    {
    }
}