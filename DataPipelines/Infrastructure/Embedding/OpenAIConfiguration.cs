namespace DataPipelines.Infrastructure.Embedding;

public class OpenAiConfiguration
{
    public required string Deployment { get; init; }
    public required string ApiKey { get; init; }
    public required string Endpoint { get; init; }
}