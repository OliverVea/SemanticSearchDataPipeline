using DataPipelines.Infrastructure;
using Microsoft.Extensions.Logging;

namespace DataPipelines.Modules;

public class AggressiveProductCleaningModule(
    ILogger<AggressiveProductCleaningModule> logger,
    AggressiveStringCleaner cleaner) : ProductCleaningModule<AggressiveStringCleaner>(logger, cleaner)
{
    public override string Name => nameof(AggressiveProductCleaningModule);
}