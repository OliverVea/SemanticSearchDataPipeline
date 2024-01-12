namespace DataPipelines.Core;

public interface IDataPipelineModule
{
    string Name { get; }
    Task ProcessAsync(CancellationToken cancellationToken);
}