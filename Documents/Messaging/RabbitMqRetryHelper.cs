using System.Collections;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace todo_be.Documents.Messaging;

public static class RabbitMqRetryHelper
{
    public static long GetRetryCount(
        IDictionary<string, object?>? headers,
        string sourceQueue)
    {
        if (headers is null
            || !headers.TryGetValue("x-death", out var deaths)
            || deaths is not IEnumerable deadLetterEvents)
        {
            return 0;
        }

        foreach (var deadLetterEvent in deadLetterEvents)
        {
            if (deadLetterEvent is not IDictionary deadLetterTable
                || GetStringValue(deadLetterTable["queue"]) != sourceQueue)
            {
                continue;
            }

            if (long.TryParse(GetStringValue(deadLetterTable["count"]), out var retryCount))
            {
                return retryCount;
            }
        }

        return 0;
    }

    public static async Task PublishToDeadLetterQueueAsync(
        IChannel channel,
        BasicDeliverEventArgs eventArgs,
        string deadLetterQueue)
    {
        var properties = new BasicProperties
        {
            ContentType = eventArgs.BasicProperties.ContentType,
            Persistent = true,
            Headers = eventArgs.BasicProperties.Headers is null
                ? null
                : new Dictionary<string, object?>(eventArgs.BasicProperties.Headers)
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: deadLetterQueue,
            mandatory: true,
            basicProperties: properties,
            body: eventArgs.Body);
    }

    private static string? GetStringValue(object? value)
    {
        return value is byte[] bytes
            ? Encoding.UTF8.GetString(bytes)
            : value?.ToString();
    }
}
