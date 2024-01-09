using KafkaFlow;
using KafkaFlow.Serializer;

using Microsoft.Extensions.Options;

using StorageService.File;
using StorageService.Messaging.Handlers;
using StorageService.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));

// Check if the directory exists
if (!Directory.Exists("/tmp"))
{
    // If the directory does not exist, create it
    Directory.CreateDirectory("/tmp");
    Console.WriteLine("Directory \"/tmp\" created successfully.");
}

builder.Services.AddSingleton<IFileWriter, FileWriter>();
builder.Services.AddSingleton<IFileReader, FileReader>();

builder.Services.AddKafka(kafka =>
{
    var kafkaSettings = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>();

    kafka
        .AddCluster(cluster => cluster
            .WithBrokers(kafkaSettings.Brokers)
            .AddConsumer(consumer =>
                consumer
                    // should all be in settings file
                    .Topic("v1.usertrackingevents")
                    .WithName("ingestor-sample-consumer")
                    .WithGroupId(kafkaSettings.ConsumerGroupId)
                    .WithBufferSize(100)
                    .WithWorkersCount(1)
                    .WithInitialState(ConsumerInitialState.Running)
                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                    .AddMiddlewares(middlewares =>
                    {
                        middlewares.AddDeserializer<NewtonsoftJsonDeserializer>();
                        middlewares.AddTypedHandlers(handlers => handlers
                            .WithHandlerLifetime(InstanceLifetime.Singleton)
                            .AddHandler<UserTrackedEventHandler>());
                    })
            )
        );
});

var app = builder.Build();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{app.Environment.EnvironmentName}.json", optional: true);

var provider = builder.Services.BuildServiceProvider();
var bus = provider.CreateKafkaBus();
await bus.StartAsync();

app.MapGet("/visits", async httpContext =>
{
    var storageSettings = httpContext.RequestServices.GetRequiredService<IOptions<StorageSettings>>().Value;
    var fileReader = httpContext.RequestServices.GetRequiredService<IFileReader>();
    
    httpContext.Response.ContentType = "text/plain";
    await using var writer = new StreamWriter(httpContext.Response.Body);
    await writer.WriteAsync(fileReader.ReadFile(storageSettings.VisitsLogFilePath));
});

app.Run();