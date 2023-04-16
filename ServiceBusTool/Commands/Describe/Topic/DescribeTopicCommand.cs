using System.CommandLine;
using System.CommandLine.Binding;
using ServiceBusTool.Bootstrapping;
using ServiceBusTool.Commands.Base;

namespace ServiceBusTool.Commands.Describe.Topic;

public class DescribeTopicCommand : GenericCommand<DescribeTopicInput>
{
    public static readonly Argument<string> TopicNameArgument = new()
    {
        Name = "TopicName",
        Description = "Name of the topic."
    };

    public DescribeTopicCommand() : base(
        name: "topic",
        description: "Describe a topic")
    {
        AddArgument(TopicNameArgument);
    }

    public override BinderBase<DescribeTopicInput> InputBinder =>
        CreateInputBinder(parseResult => new DescribeTopicInput(
            Namespace: parseResult.GetValueForOption(ServiceBusToolCommand.NamespaceOption),
            Topic: parseResult.GetValueForArgument(TopicNameArgument)));
}