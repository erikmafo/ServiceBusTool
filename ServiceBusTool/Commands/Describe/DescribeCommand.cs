using System.CommandLine;
using ServiceBusTool.Bootstrapping;
using ServiceBusTool.Commands.Describe.Queue;
using ServiceBusTool.Commands.Describe.Subscription;
using ServiceBusTool.Commands.Describe.Topic;

namespace ServiceBusTool.Commands.Describe;

public class DescribeCommand : Command
{
    public DescribeCommand() : base(
        name: "describe",
        description: "Display runtime info about a topic, subscription or queue")
    {
        Topic = new DescribeTopicCommand();
        Queue = new DescribeQueueCommand();
        Subscription = new DescribeSubscriptionCommand();
        
        AddCommand(Topic);
        AddCommand(Queue);
        AddCommand(Subscription);
    }

    public DescribeTopicCommand Topic { get; }

    public DescribeQueueCommand Queue { get; }

    public DescribeSubscriptionCommand Subscription { get; }
}