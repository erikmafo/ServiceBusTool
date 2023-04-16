using System.CommandLine;
using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Describe.Topic;

public class DescribeTopicHandler : GenericHandlerBase<DescribeTopicInput>
{
    private readonly IServiceBusHelper _serviceBus;

    public DescribeTopicHandler(IServiceBusHelper serviceBus)
    {
        _serviceBus = serviceBus;
    }

    public override async Task Handle(
        DescribeTopicInput input,
        CancellationToken token = default)
    {
        var topicProperties = await _serviceBus.GetTopicPropertiesAsync(input.Namespace, input.Topic, token);
        var runtimeProperties = await _serviceBus.GetTopicRuntimePropertiesAsync(input.Namespace, input.Topic, token);

        Console.WriteLine($"{nameof(topicProperties.Status)}: \t {topicProperties.Status}");
        Console.WriteLine($"{nameof(topicProperties.EnablePartitioning)}: \t {topicProperties.EnablePartitioning}");
        Console.WriteLine($"{nameof(topicProperties.SupportOrdering)}: \t {topicProperties.SupportOrdering}");
        Console.WriteLine($"{nameof(topicProperties.MaxSizeInMegabytes)}: \t {topicProperties.MaxSizeInMegabytes}");
        Console.WriteLine($"{nameof(runtimeProperties.AccessedAt)}: \t {runtimeProperties.AccessedAt}");
        Console.WriteLine($"{nameof(runtimeProperties.CreatedAt)}: \t {runtimeProperties.CreatedAt}");
        Console.WriteLine($"{nameof(runtimeProperties.UpdatedAt)}: \t {runtimeProperties.UpdatedAt}");
        Console.WriteLine($"{nameof(runtimeProperties.ScheduledMessageCount)}: \t {runtimeProperties.ScheduledMessageCount}");
        Console.WriteLine($"{nameof(runtimeProperties.SubscriptionCount)}: \t {runtimeProperties.SubscriptionCount}");
        Console.WriteLine($"{nameof(runtimeProperties.SizeInBytes)}: \t {runtimeProperties.SizeInBytes}");
    }
}