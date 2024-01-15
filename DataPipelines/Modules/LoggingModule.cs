using DataPipelines.Core;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class LoggingModule<T>(ILogger<LoggingModule<T>> logger) : IDataPipelineModule
{
    private const int BatchSize = int.MaxValue;
    
    public DataPipe<T>? InputPipe { get; set; }
    public Func<T, int, string> Stringify { get; set; } = (item, _) => item?.ToString() ?? string.Empty;

    public string Name => nameof(LoggingModule<T>);
    public bool Finished { get; private set; }

    private int _i;

    public Task ProcessAsync(CancellationToken cancellationToken)
    {
        Process();
        return Task.CompletedTask;
    }

    private void Process()
    {
        if (InputPipe is not { } inputPipe) return;
        
        var input = inputPipe.Dequeue(BatchSize);
        
        foreach (var item in input) Log(item);
        
        if (inputPipe.FinishedInput) Finished = true;
    }

    private void Log(T item)
    {
        var message = Stringify(item, _i++);
        if (string.IsNullOrWhiteSpace(message)) return;
        
        logger.LogInformation(message);
    }
}