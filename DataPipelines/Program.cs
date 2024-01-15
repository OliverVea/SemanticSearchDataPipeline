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

const string query = "Phillips skruetrækker til amatører";

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
serviceCollection.AddSingleton<CosineSimilarityCalculator>();
serviceCollection.AddSingleton<ITextEmbeddingClient, OpenAIEmbeddingClient>();
serviceCollection.AddSingleton<TextEmbeddingCache>();
serviceCollection.AddSingleton<TextEmbeddingStore>();
serviceCollection.AddSingleton<TextEmbeddingRepository>();
serviceCollection.AddSingleton<OpenAIEmbeddingClient>();
serviceCollection.AddSingleton<IProductLoader, JsonProductLoader>();
serviceCollection.AddSingleton<IHashProvider, Sha256HashProvider>();
serviceCollection.AddMemoryCache();

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

var productDataCollectionModule = pipeline.AddModule<CollectionModule<ProductData>>();
productDataCollectionModule.InputPipe = new DataPipe<ProductData>();
productDataInput.OutputPipes.Add(productDataCollectionModule.InputPipe);

var distinctProductModule = pipeline.AddModule<DistinctProductModule>();
distinctProductModule.InputPipe = new DataPipe<ProductData>();
productDataInput.OutputPipes.Add(distinctProductModule.InputPipe);
distinctProductModule.OutputPipe = new DataPipe<ProductData>();

var productCleaningModule = pipeline.AddModule<AggressiveProductCleaningModule>();
productCleaningModule.InputPipe = distinctProductModule.OutputPipe;
productCleaningModule.OutputPipe = new DataPipe<ProductData>();

var simplifiedYmlTransformerModule = pipeline.AddModule<SimplifiedYmlTransformerModule>();
simplifiedYmlTransformerModule.InputPipe = productCleaningModule.OutputPipe;
simplifiedYmlTransformerModule.OutputPipe = new DataPipe<TextData>();

var openAiAda2EmbedderModule = pipeline.AddModule<EmbedderModule>();
openAiAda2EmbedderModule.InputPipe = simplifiedYmlTransformerModule.OutputPipe;
openAiAda2EmbedderModule.OutputPipe = new DataPipe<EmbeddingData>();

var collectionModule = pipeline.AddModule<CollectionModule<EmbeddingData>>();
collectionModule.InputPipe = openAiAda2EmbedderModule.OutputPipe;

Console.WriteLine("Starting embedding pipeline.");

var cancellationTokenSource = new CancellationTokenSource();
await pipeline.RunAsync(cancellationTokenSource.Token);

var embeddingData = collectionModule.Collection.ToArray();
var productData = productDataCollectionModule.Collection.ToArray();
var productDataBySkuId = productData.ToDictionary(p => p.SkuId, p => p);

Console.WriteLine($"Embedding pipeline finished. {embeddingData.Length} embeddings generated.");

var embedder = serviceProvider.GetRequiredService<ITextEmbeddingClient>();

while (!cancellationTokenSource.IsCancellationRequested)
{
    pipeline.Clear();
    
    Console.WriteLine("Enter search query: (q to quit)");
    var input = Console.ReadLine();
    if (input is null) continue;
    
    if (input == "q")
    {
        cancellationTokenSource.Cancel();
        break;
    }
    
    var rawQueryEmbeddings = await embedder.GetEmbeddingsAsync(new [] { input }, CancellationToken.None);
    var queryEmbedding = new QueryEmbedding
    {
        Query = input,
        Embedding = rawQueryEmbeddings.First()
    };

    var inputPipe = new DataPipe<EmbeddingData>();
    inputPipe.Enqueue(embeddingData);
    inputPipe.FinishedInput = true;
    
    var similarityModule = pipeline.AddModule<SimilarityModule<CosineSimilarityCalculator>>();
    similarityModule.InputPipe = inputPipe;
    similarityModule.OutputPipe = new DataPipe<SimilarityReport>();
    similarityModule.QueryEmbedding = queryEmbedding;

    var reportFormattingModule = pipeline.AddModule<ReportFormattingModule>();
    reportFormattingModule.InputPipe = similarityModule.OutputPipe;
    reportFormattingModule.ProductData = productDataBySkuId;
    reportFormattingModule.OutputPipe = new DataPipe<string>();

    var loggingModule = pipeline.AddModule<LoggingModule<string>>();
    loggingModule.InputPipe = reportFormattingModule.OutputPipe;
    
    await pipeline.RunAsync(cancellationTokenSource.Token);
}