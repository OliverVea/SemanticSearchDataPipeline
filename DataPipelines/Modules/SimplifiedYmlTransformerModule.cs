using DataPipelines.Infrastructure.Templating.SimplifiedYml;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class SimplifiedYmlTransformerModule(
    ILogger<SimplifiedYmlTransformer> logger,
    SimplifiedYmlTransformer transformer) : TextTransformerModule<SimplifiedYmlTransformer>(logger, transformer);