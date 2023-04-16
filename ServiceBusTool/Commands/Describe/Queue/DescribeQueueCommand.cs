using System.CommandLine;
using System.CommandLine.Binding;
using ServiceBusTool.Commands.Base;

namespace ServiceBusTool.Commands.Describe.Queue;

public class DescribeQueueCommand : GenericCommand<DescribeQueueInput>
{
    public static readonly Argument<string> QueueArgument = new()
    {
        Name = "QueueName",
        Description = "The name of the queue."
    };
    
    public DescribeQueueCommand() : base(
        name: "queue",
        description: "Describe a queue.")
    {
        AddArgument(QueueArgument);
    }

    public override BinderBase<DescribeQueueInput> InputBinder => 
        CreateInputBinder(parseResult => 
            new DescribeQueueInput(
                Namespace: parseResult.GetValueForOption(ServiceBusToolCommand.NamespaceOption), 
                Queue: parseResult.GetValueForArgument(QueueArgument)));
}