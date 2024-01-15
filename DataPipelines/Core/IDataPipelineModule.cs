namespace DataPipelines.Core;

public interface IDataPipelineModule
{
    string Name { get; }
    bool Finished { get; }
    Task ProcessAsync(CancellationToken cancellationToken);
}