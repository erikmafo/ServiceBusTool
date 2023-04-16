using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusTool.ServiceBus;

public interface IServiceBusHelper
{
    Task<TopicProperties> GetTopicPropertiesAsync(
        string sbNamespace, 
        string topic,
        CancellationToken token = default);
    
    Task<TopicRuntimeProperties> GetTopicRuntimePropertiesAsync(
        string sbNamespace, 
        string topic, 
        CancellationToken token = default);

    Task<SubscriptionProperties> GetSubscriptionPropertiesAsync(
        string sbNamespace, 
        string topic,
        string subscription,
        CancellationToken token = default);
    
    Task<SubscriptionRuntimeProperties> GetSubscriptionRuntimePropertiesAsync(
        string sbNamespace, 
        string topic,
        string subscription,
        CancellationToken token = default);
    
    Task<QueueProperties> GetQueuePropertiesAsync(
        string sbNamespace,
        string queue,
        CancellationToken token = default);
    
    Task<QueueRuntimeProperties> GetQueueRuntimePropertiesAsync(
        string sbNamespace, 
        string queue,
        CancellationToken token = default);

    Task ReceiveMessagesAsync(
        string sbNamespace,
        string queue,
        string subQueue,
        IMessageHandler messageHandler,
        ReceiveOptions receiveOptions,
        CancellationToken token = default);

    Task ReceiveMessagesAsync(
        string sbNamespace,
        string topic,
        string subscription,
        string subQueue,
        IMessageHandler messageHandler,
        ReceiveOptions receiveOptions,
        CancellationToken token = default);

    Task TransferMessagesAsync(
        string sbNamespace, 
        string queue, 
        string subQueue, 
        string targetEntity,
        IProcessMessagesReporter reporter,
        CancellationToken token = default);
    
    Task TransferMessagesAsync(
        string sbNamespace, 
        string topic, 
        string subscription, 
        string subQueue, 
        string targetEntity,
        IProcessMessagesReporter reporter,
        CancellationToken token = default);
    
    Task PurgeMessagesAsync(
        string sbNamespace, 
        string queue, 
        string subQueue,
        IProcessMessagesReporter reporter,
        CancellationToken token = default);
    
    Task PurgeMessagesAsync(
        string sbNamespace, 
        string topic, 
        string subscription, 
        string subQueue,
        IProcessMessagesReporter reporter,
        CancellationToken token = default);
}