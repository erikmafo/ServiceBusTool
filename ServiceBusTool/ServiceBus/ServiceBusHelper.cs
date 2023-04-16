using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusTool.ServiceBus;

public class ServiceBusHelper : IServiceBusHelper
{
    private readonly TokenCredential _tokenCredential;
    
    private ServiceBusHelper(TokenCredential tokenCredential)
    {
        _tokenCredential = tokenCredential;
    }
    
    public ServiceBusHelper() : this(new InteractiveBrowserCredential()) { }

    public async Task ReceiveMessagesAsync(     
        string sbNamespace, 
        string queue,
        string subQueue,
        IMessageHandler messageHandler,
        ReceiveOptions receiveOptions,
        CancellationToken token = default)
    {
        await using var client = GetClient(sbNamespace);
        await using var receiver = client.CreateReceiver(
            queue, ServiceBusReceiverOptions(subQueue, Enum.Parse<ServiceBusReceiveMode>(receiveOptions.ReceiveMode)));
        
        await ReceiveMessagesAsync(receiver, receiveOptions, messageHandler, token);
    }

    public async Task ReceiveMessagesAsync(
        string sbNamespace, 
        string topic, 
        string subscription,
        string subQueue,
        IMessageHandler messageHandler, 
        ReceiveOptions receiveOptions,
        CancellationToken token = default)
    {
        await using var client = GetClient(sbNamespace);
        await using var receiver = client.CreateReceiver(
            topic, subscription, ServiceBusReceiverOptions(subQueue, Enum.Parse<ServiceBusReceiveMode>(receiveOptions.ReceiveMode)));
        
        await ReceiveMessagesAsync(receiver, receiveOptions, messageHandler, token);
    }

    public async Task<TopicProperties> GetTopicPropertiesAsync(
        string sbNamespace, 
        string topic,
        CancellationToken token = default)
    {
        var client = GetAdministrationClient(sbNamespace);
        var response = await client.GetTopicAsync(topic, token);
        return response.Value;
    }

    public async Task<TopicRuntimeProperties> GetTopicRuntimePropertiesAsync(
        string sbNamespace, 
        string topic, 
        CancellationToken token = default)
    {
        var client = GetAdministrationClient(sbNamespace);
        var response = await client.GetTopicRuntimePropertiesAsync(topic, token);
        return response.Value;
    }

    public async Task<SubscriptionProperties> GetSubscriptionPropertiesAsync(string sbNamespace, string topic, string subscription,
        CancellationToken token = default)
    {
        var client = GetAdministrationClient(sbNamespace);
        var response = await client.GetSubscriptionAsync(topic, subscription, token);
        return response.Value;
    }

    public async Task<SubscriptionRuntimeProperties> GetSubscriptionRuntimePropertiesAsync(string sbNamespace, string topic, string subscription,
        CancellationToken token = default)
    {
        var client = GetAdministrationClient(sbNamespace);
        var response = await client.GetSubscriptionRuntimePropertiesAsync(topic, subscription, token);
        return response.Value;
    }

    public async Task<QueueProperties> GetQueuePropertiesAsync(string sbNamespace, string queue, CancellationToken token = default)
    {
        var client = GetAdministrationClient(sbNamespace);
        var response = await client.GetQueueAsync(queue, token);
        return response.Value;
    }

    public async Task<QueueRuntimeProperties> GetQueueRuntimePropertiesAsync(string sbNamespace, string queue, CancellationToken token = default)
    {
        var client = GetAdministrationClient(sbNamespace);
        var response = await client.GetQueueRuntimePropertiesAsync(queue, token);
        return response.Value;
    }

    public async Task TransferMessagesAsync(
        string sbNamespace, 
        string queue,
        string subQueue,
        string targetEntity,
        IProcessMessagesReporter reporter,
        CancellationToken token = default)
    {
        await using var client = GetClient(sbNamespace);
        await using var receiver = client.CreateReceiver(queue, ServiceBusReceiverOptions(subQueue));
        await using var sender = client.CreateSender(targetEntity);
        var totalMessages = await GetTotalMessagesAsync(sbNamespace, queue, subQueue, token);
        await TransferMessagesAsync(receiver, sender, totalMessages, reporter, token);
    }

    public async Task TransferMessagesAsync(
        string sbNamespace, 
        string topic,
        string subscription,
        string subQueue,
        string targetEntity,
        IProcessMessagesReporter reporter,
        CancellationToken token = default)
    {
        await using var client = GetClient(sbNamespace);
        await using var receiver = client.CreateReceiver(topic, subscription, ServiceBusReceiverOptions(subQueue));
        await using var sender = client.CreateSender(targetEntity);
        var totalMessages = await GetTotalMessagesAsync(sbNamespace, topic, subscription, subQueue, token);
        await TransferMessagesAsync(receiver, sender, totalMessages, reporter, token);
    }

    public async Task PurgeMessagesAsync(string sbNamespace, string queue, string subQueue, IProcessMessagesReporter reporter,
        CancellationToken token = default)
    {
        await using var client = GetClient(sbNamespace);
        await using var receiver = client.CreateReceiver(queue, ServiceBusReceiverOptions(subQueue, ServiceBusReceiveMode.ReceiveAndDelete));
        var totalMessages = await GetTotalMessagesAsync(sbNamespace, queue, subQueue, token);
        await PurgeMessagesAsync(receiver, totalMessages, reporter, token);
    }

    public async Task PurgeMessagesAsync(string sbNamespace, string topic, string subscription, string subQueue,
        IProcessMessagesReporter reporter, CancellationToken token = default)
    {
        await using var client = GetClient(sbNamespace);
        await using var receiver = client.CreateReceiver(topic, subscription, ServiceBusReceiverOptions(subQueue, ServiceBusReceiveMode.ReceiveAndDelete));
        var totalMessages = await GetTotalMessagesAsync(sbNamespace, topic, subscription, subQueue, token);
        await PurgeMessagesAsync(receiver, totalMessages, reporter, token);
    }

    private static async Task ReceiveMessagesAsync(
        ServiceBusReceiver receiver,
        ReceiveOptions options,
        IMessageHandler messageHandler, 
        CancellationToken token)
    {
        var maxMessages = options.MaxMessages;
        var receivedMessages = 0;

        while (!token.IsCancellationRequested && (maxMessages == null || maxMessages > receivedMessages))
        {
            var message = await receiver.ReceiveMessageAsync(options.MaxWaitTime, token);
            messageHandler.ReceiveMessage(message);
            receivedMessages++;
            if (options.Acknowledge)
            {
                await receiver.CompleteMessageAsync(message, token);
            }
        }
    }

    private async Task TransferMessagesAsync(
        ServiceBusReceiver receiver, 
        ServiceBusSender sender, 
        long totalMessages,
        IProcessMessagesReporter reporter, 
        CancellationToken token)
    {
        Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessagesAsync() => 
            receiver.ReceiveMessagesAsync(maxMessages: 1000, maxWaitTime: TimeSpan.FromMinutes(1), token);

        Task SendMessagesAsync(IEnumerable<ServiceBusReceivedMessage> messages) =>
            sender.SendMessagesAsync(messages.Select(msg => new ServiceBusMessage(msg)), token);

        Task CompleteMessagesAsync(IEnumerable<ServiceBusReceivedMessage> messages) =>
            Task.WhenAll(messages.Select(msg => receiver.CompleteMessageAsync(msg, token)));

        var totalMessagesProcessed = 0;
        
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages;

        do
        {
            receivedMessages = await ReceiveMessagesAsync();
            await SendMessagesAsync(receivedMessages);
            await CompleteMessagesAsync(receivedMessages);
            totalMessagesProcessed += receivedMessages.Count;
            reporter.ReportProgress(new ProcessedMessagesStatistics(totalMessages, totalMessagesProcessed));
            
        } 
        while (receivedMessages.Any());
    }
    
    private async Task PurgeMessagesAsync(
        ServiceBusReceiver receiver,
        long totalMessages,
        IProcessMessagesReporter reporter, 
        CancellationToken token)
    {
        Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessagesAsync() => 
            receiver.ReceiveMessagesAsync(maxMessages: 1000, maxWaitTime: TimeSpan.FromMinutes(1), token);

        var totalMessagesProcessed = 0;
        
        IReadOnlyList<ServiceBusReceivedMessage> receivedMessages;

        do
        {
            receivedMessages = await ReceiveMessagesAsync();
            totalMessagesProcessed += receivedMessages.Count;
            reporter.ReportProgress(new ProcessedMessagesStatistics(totalMessages, totalMessagesProcessed));
            
        } 
        while (receivedMessages.Any());
    }

    private async Task<long> GetTotalMessagesAsync(
        string sbNamespace, string topic, string subscription, string subQueue, CancellationToken token)
    {
        var runtimeProperties = await GetSubscriptionRuntimePropertiesAsync(sbNamespace, topic, subscription, token);
        return subQueue switch
        {
            "DeadLetter" => runtimeProperties.DeadLetterMessageCount,
            "TransferDeadLetter" => runtimeProperties.TransferDeadLetterMessageCount,
            _ => runtimeProperties.ActiveMessageCount,
        };
    }
    
    private async Task<long> GetTotalMessagesAsync(
        string sbNamespace, string queue, string subQueue, CancellationToken token)
    {
        var runtimeProperties = await GetQueueRuntimePropertiesAsync(sbNamespace, queue, token);
        return subQueue switch
        {
            "DeadLetter" => runtimeProperties.DeadLetterMessageCount,
            "TransferDeadLetter" => runtimeProperties.TransferDeadLetterMessageCount,
            _ => runtimeProperties.ActiveMessageCount,
        };
    }

    private ServiceBusAdministrationClient GetAdministrationClient(string sbNamespace) =>
        new(sbNamespace, _tokenCredential);

    private ServiceBusClient GetClient(string sbNamespace) =>
        new(sbNamespace, _tokenCredential);

    private static ServiceBusReceiverOptions ServiceBusReceiverOptions(
        string subQueue, 
        ServiceBusReceiveMode receiveMode = ServiceBusReceiveMode.PeekLock,
        int prefectCount = 1000) =>
        new()
        {
            ReceiveMode = receiveMode,
            SubQueue = string.IsNullOrEmpty(subQueue) ? SubQueue.None : Enum.Parse<SubQueue>(subQueue),
            PrefetchCount = prefectCount
        };
}