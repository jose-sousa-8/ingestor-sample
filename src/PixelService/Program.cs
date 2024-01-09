using System.Net;

using Confluent.Kafka;

using KafkaFlow;
using KafkaFlow.Serializer;

using PixelService.Images;

using PixelService.Settings;

using TrackingContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGifImageGenerator, GifImageGenerator>();

builder.Services.AddKafka(kafka =>
{
    var kafkaSettings = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>();

    kafka
        .AddCluster(cluster => cluster
            .WithBrokers(kafkaSettings.Brokers)
            .AddProducer<UserTrackedEvent>(producerConfigurator =>
            {
                producerConfigurator.DefaultTopic("v1.usertrackingevents");
                producerConfigurator
                    .AddMiddlewares(middlewares =>
                    {
                        middlewares.AddSerializer<NewtonsoftJsonSerializer>();
                    });
            })
        );
});

var app = builder.Build();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{app.Environment.EnvironmentName}.json", optional: true);

app.MapGet("/track", async httpContext =>
{
    // assuming the request doest not go through a proxy
    var ipAddress = httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    if (string.IsNullOrWhiteSpace(ipAddress))
    {
        // Arguable
        httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
        return;
    }
    
    var userTrackedEventProducer = httpContext.RequestServices.GetRequiredService<IMessageProducer<UserTrackedEvent>>();
    var gifImageGenerator = httpContext.RequestServices.GetRequiredService<IGifImageGenerator>();

    var userTrackedEvent = new UserTrackedEvent(ipAddress)
    {
        Timestamp = DateTimeOffset.UtcNow,
        UserAgent = httpContext.Request?.Headers?["User-Agent"] ?? string.Empty,
        Referer = httpContext.Request?.Headers?["Referer"] ?? string.Empty
    };
    
    // Could be executed in hangfire as a retriable job and to be executed asynchronously
    Task.Run(async () =>
    {
        var deliveryResult = await userTrackedEventProducer.ProduceAsync(Guid.NewGuid().ToString(), userTrackedEvent);   
        // could handle failure
        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
        {
            Console.WriteLine($"produced userTrackedEvent ip:{userTrackedEvent.IpAddress}");
        }
    });
    
    httpContext.Response.ContentType = "image/gif";
    await httpContext.Response.Body.WriteAsync(gifImageGenerator.Generate().AsMemory(0, 1));
});

app.Run();