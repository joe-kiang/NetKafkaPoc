using MassTransit;
using PocConsumer.Models;

namespace PocConsumer
{
    public class PocMessageConsumer : IConsumer<PocMessage>
    {
        public async Task Consume(ConsumeContext<PocMessage> context)
        {
            var messageContent = context.Message.Content;
            Console.WriteLine($"Received message: {messageContent}");
        }
    }
}