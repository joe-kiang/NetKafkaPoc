using MassTransit;
using PocApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
    
    x.AddRider(rider =>
    {
        rider.AddProducer<PocMessage>(builder.Configuration["KAFKA_TOPIC"]);

        rider.UsingKafka((context, k) =>
        {
            k.Host(builder.Configuration["KAFKA_BROKER"]);
        });
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var messageCount = 0;

app.MapPost("/messages", async (ITopicProducer<PocMessage> producer, CancellationToken cancellationToken) =>
{
    var message = new PocMessage
    {
        Content = $"test-{++messageCount}"
    };

    await producer.Produce(message, cancellationToken);
    return Results.Ok($"Message sent: {message.Content}");
});

app.Run();