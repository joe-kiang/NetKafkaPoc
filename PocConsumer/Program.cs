using MassTransit;
using PocConsumer;
using PocConsumer.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
    
    x.AddRider(rider =>
    {
        rider.AddConsumer<PocMessageConsumer>();
        
        rider.UsingKafka((context, k) =>
        {
            k.Host(builder.Configuration["KAFKA_BROKER"]); 
            k.TopicEndpoint<PocMessage>(builder.Configuration["KAFKA_TOPIC"], "consumer-group-id", e =>
            {
                e.ConfigureConsumer<PocMessageConsumer>(context);
            });
        });
    });
});

var host = builder.Build();
await host.RunAsync();