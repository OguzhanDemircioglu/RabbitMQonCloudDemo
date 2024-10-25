using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CatchMessage;

internal class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqps://uydkmrcy:eVwCFhNxrZ6BQDSgIJuMEr6ZGEpH6mwB@kangaroo.rmq.cloudamqp.com/uydkmrcy")
        };

        await using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "test-queue", durable: true, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, @event) =>
        {
            var message = Encoding.UTF8.GetString(@event.Body.ToArray());
            Console.WriteLine("Gelen Mesaj: " + message);
            await Task.Yield(); 
        };

        await channel.BasicConsumeAsync(queue: "test-queue", autoAck: true, consumer: consumer);

        Console.WriteLine("Mesajlar dinleniyor...");
        Console.ReadLine();
    }
}