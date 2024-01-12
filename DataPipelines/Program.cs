using DataPipelines.Core;
using DataPipelines.Infrastructure;
using DataPipelines.Infrastructure.Caching;
using DataPipelines.Infrastructure.Embedding;
using DataPipelines.Infrastructure.Loading;
using DataPipelines.Infrastructure.Templating.SimplifiedYml;
using DataPipelines.Models;
using DataPipelines.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("Data/appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection();

var openAiConfiguration = configuration.GetRequiredSection("OpenAI").Get<OpenAiConfiguration>();
if (openAiConfiguration is null) throw new Exception("OpenAI configuration was null");

serviceCollection.AddSingleton<DataPipeline>();
serviceCollection.AddSingleton<AggressiveStringCleaner>();
serviceCollection.AddSingleton<SimplifiedYmlTransformer>();
serviceCollection.AddSingleton<OpenAIEmbedder>();
serviceCollection.AddSingleton<IProductLoader, JsonProductLoader>();
serviceCollection.AddSingleton<IHashProvider, Sha256HashProvider>();
serviceCollection.AddOpenAi(openAiConfiguration);

serviceCollection.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddConfiguration(configuration.GetRequiredSection("Logging"));
});

var serviceProvider = serviceCollection.BuildServiceProvider();

var pipeline = serviceProvider.GetRequiredService<DataPipeline>();

var productDataInput = pipeline.AddModule<SkuProductJsonLoadingModule>();
productDataInput.ProductFile = new FileInfo("Data/davidsen-products.json");
productDataInput.SkuFile = new FileInfo("Data/davidsen-skus.json");
productDataInput.OutputPipe = new DataPipe<ProductData>();

var distinctProductModule = pipeline.AddModule<DistinctProductModule>();
distinctProductModule.InputPipe = productDataInput.OutputPipe;
distinctProductModule.OutputPipe = new DataPipe<ProductData>();

var productCleaningModule = pipeline.AddModule<AggressiveProductCleaningModule>();
productCleaningModule.InputPipe = distinctProductModule.OutputPipe;
productCleaningModule.OutputPipe = new DataPipe<ProductData>();

var simplifiedYmlTransformerModule = pipeline.AddModule<SimplifiedYmlTransformerModule>();
simplifiedYmlTransformerModule.InputPipe = productCleaningModule.OutputPipe;
simplifiedYmlTransformerModule.OutputPipe = new DataPipe<TextData>();

var openAiAda2EmbedderModule = pipeline.AddModule<OpenAIEmbedderModule>();
openAiAda2EmbedderModule.InputPipe = simplifiedYmlTransformerModule.OutputPipe;
openAiAda2EmbedderModule.OutputPipe = new DataPipe<EmbeddingData>();

var loggingModule = pipeline.AddModule<LoggingModule<EmbeddingData>>();
loggingModule.InputPipe = openAiAda2EmbedderModule.OutputPipe;
loggingModule.Stringify = (textData, i) => $"{i}: {textData.SkuId}";

await pipeline.RunAsync(CancellationToken.None);

    /*
    .LoadData(productFile, skuFile)
    .ApplyProductDataTransform<DistinctProductFilter>()
    .ApplyProductDataTransform<ProductDataCleaner>()
    .TransformProducts<SimplifiedYmlTransformer>([])
    //.ApplyTextTransform<DeepLTextTranslator>() Too Expensive to run :(
    .EmbedTexts<OpenAIEmbedder>()
    .EvaluateOnQueriesAsync(queryEmbeddings);
    */
