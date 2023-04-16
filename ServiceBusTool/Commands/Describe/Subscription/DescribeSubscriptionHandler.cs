using System.CommandLine;
using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Describe.Subscription;

public class DescribeSubscriptionHandler : GenericHandlerBase<DescribeSubscriptionInput>
{
    private readonly IServiceBusHelper _serviceBus;

    public DescribeSubscriptionHandler(IServiceBusHelper serviceBus)
    {
        _serviceBus = serviceBus;
    }

    public override async Task Handle(DescribeSubscriptionInput input, CancellationToken token = default)
    {
        var entityPath = EntityPath.Parse(input.EntityPath);
        
        var properties = await _serviceBus.GetSubscriptionPropertiesAsync(input.Namespace, entityPath.Topic, entityPath.Subscription, token);
        var runtimeProperties = await _serviceBus.GetSubscriptionRuntimePropertiesAsync(input.Namespace, entityPath.Topic, entityPath.Subscription, token);
        
        Console.WriteLine($"{nameof(properties.TopicName)}: \t {properties.TopicName}");
        Console.WriteLine($"{nameof(properties.SubscriptionName)}: \t {properties.TopicName}");
        Console.WriteLine($"{nameof(properties.Status)}: \t {properties.Status}");
        Console.WriteLine($"{nameof(properties.ForwardTo)}: \t {properties.ForwardTo}");
        Console.WriteLine($"{nameof(properties.LockDuration)}: \t {properties.LockDuration}");
        Console.WriteLine($"{nameof(properties.RequiresSession)}: \t {properties.RequiresSession}");
        Console.WriteLine($"{nameof(properties.UserMetadata)}: \t {properties.UserMetadata}");
        Console.WriteLine($"{nameof(properties.EnableBatchedOperations)}: \t {properties.EnableBatchedOperations}");
        Console.WriteLine($"{nameof(properties.MaxDeliveryCount)}: \t {properties.MaxDeliveryCount}");
        Console.WriteLine($"{nameof(properties.AutoDeleteOnIdle)}: \t {properties.AutoDeleteOnIdle}");
        Console.WriteLine($"{nameof(properties.DeadLetteringOnMessageExpiration)}: \t {properties.DeadLetteringOnMessageExpiration}");
        Console.WriteLine($"{nameof(properties.DefaultMessageTimeToLive)}: \t {properties.DefaultMessageTimeToLive}");
        Console.WriteLine($"{nameof(properties.ForwardDeadLetteredMessagesTo)}: \t {properties.ForwardDeadLetteredMessagesTo}");
        Console.WriteLine($"{nameof(properties.EnableDeadLetteringOnFilterEvaluationExceptions)}: \t {properties.EnableDeadLetteringOnFilterEvaluationExceptions}");
        
        Console.WriteLine($"{nameof(runtimeProperties.AccessedAt)}: \t {runtimeProperties.AccessedAt}");
        Console.WriteLine($"{nameof(runtimeProperties.CreatedAt)}: \t {runtimeProperties.CreatedAt}");
        Console.WriteLine($"{nameof(runtimeProperties.UpdatedAt)}: \t {runtimeProperties.UpdatedAt}");
        Console.WriteLine($"{nameof(runtimeProperties.ActiveMessageCount)}: \t {runtimeProperties.ActiveMessageCount}");
        Console.WriteLine($"{nameof(runtimeProperties.DeadLetterMessageCount)}: \t {runtimeProperties.DeadLetterMessageCount}");
        Console.WriteLine($"{nameof(runtimeProperties.TransferMessageCount)}: \t {runtimeProperties.TransferMessageCount}");
        Console.WriteLine($"{nameof(runtimeProperties.TransferDeadLetterMessageCount)}: \t {runtimeProperties.TransferDeadLetterMessageCount}");
        Console.WriteLine($"{nameof(runtimeProperties.TotalMessageCount)}: \t {runtimeProperties.TotalMessageCount}");
    }
}