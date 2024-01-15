using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;

namespace DataPipelines.Infrastructure.Embedding;

public static class EmbeddingServiceExtensions
{
    public static void AddOpenAi(this IServiceCollection services, OpenAiConfiguration configuration)
    {
        var endpoint = new Uri(configuration.Endpoint);
        var credential = new AzureKeyCredential(configuration.ApiKey);
        
        var client = new OpenAIClient(endpoint, credential);
        
        services.AddSingleton(configuration);
        services.AddSingleton(client);
        
        services.AddSingleton<ITextEmbeddingClient, OpenAIEmbeddingClient>();
    }
}