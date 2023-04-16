using System.CommandLine;
using System.CommandLine.Binding;
using ServiceBusTool.Bootstrapping;
using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Describe.Subscription;

public class DescribeSubscriptionCommand : GenericCommand<DescribeSubscriptionInput>
{
    public static readonly Argument<string> SubscriptionPathArgument = new()
    {
        Name = "SubscriptionPath",
        Description = "The path to the subscription. Valid format: {topic}/Subscriptions/{subscription}."
    };

    public DescribeSubscriptionCommand() : base(
        name: "subscription",
        description: "Describe a subscription")
    {
        AddArgument(SubscriptionPathArgument);
        AddValidator(commandResult =>
        {
            var entityPath = commandResult.GetValueForArgument(SubscriptionPathArgument);
            if (!EntityPath.IsValidSubscription(entityPath, out var errorMessage))
            {
                commandResult.ErrorMessage = errorMessage;
            }
        });
    }

    public override BinderBase<DescribeSubscriptionInput> InputBinder =>
        CreateInputBinder(parseResult => new DescribeSubscriptionInput(
            Namespace: parseResult.GetValueForOption(ServiceBusToolCommand.NamespaceOption),
            EntityPath: parseResult.GetValueForArgument(SubscriptionPathArgument)));
}