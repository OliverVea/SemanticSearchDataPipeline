using DataPipelines.Infrastructure.Embedding;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class OpenAIEmbedderModule(ILogger<OpenAIEmbedderModule> logger, OpenAIEmbedder textEmbedder) : EmbedderModule<OpenAIEmbedder>(logger, textEmbedder)
{
    public override string Name => nameof(OpenAIEmbedderModule);
}