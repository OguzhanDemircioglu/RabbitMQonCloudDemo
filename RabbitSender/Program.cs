using System.Text;
using RabbitMQ.Client;

namespace RabbitSender;

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

        Console.WriteLine("RabbitMQ kuyruğuna mesaj göndermek için metin girin ('exit' yazarak çıkabilirsiniz):");

        while (true)
        {
            var mesaj = Console.ReadLine();

            if (mesaj?.ToLower() == "exit")
                break;

            var body = Encoding.UTF8.GetBytes(mesaj!);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "test-queue",
                mandatory: false,
                body: new ReadOnlyMemory<byte>(body),
                cancellationToken: CancellationToken.None
            );

            Console.WriteLine("Mesaj gönderildi: " + mesaj);
        }

        Console.WriteLine("Program sonlandırıldı.");
    }
}