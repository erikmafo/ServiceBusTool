using System.CommandLine;
using System.CommandLine.Binding;
using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Transfer;

public class TransferCommand : GenericCommand<TransferInput>
{
    public static readonly Argument<string> SourcePathArgument = 
        new() { 
            Name = "SourcePath",
            Description = 
                @"Path to the entity, (queue or subscription), to read messages from, valid formats:
    - {queue}
    - {queue}/${subQueue} 
    - {topic}/Subscriptions/{subscription}
    - {topic}/Subscriptions/{subscription}
    - {topic}/Subscriptions/{subscription}/${subQueue}" };

    public static readonly Argument<string> TargetQueueOrTopic = 
        new()
        {
            Name = "TargetQueueOrTopic",
            Description = "The name of the queue or topic to send messages to."
        };
    
    public TransferCommand() : base(
        name: "transfer",
        description: "Transfer messages from a queue or subscription to a queue or topic.")
    {
        AddArgument(SourcePathArgument);
        AddArgument(TargetQueueOrTopic);
        AddValidator(commandResult =>
        {
            var sourcePath = commandResult.GetValueForArgument(SourcePathArgument);
            if (!EntityPath.IsValid(sourcePath, out var errorMessage))
            {
                commandResult.ErrorMessage = errorMessage;
            }
        });
    }

    public override BinderBase<TransferInput> InputBinder =>
        CreateInputBinder(parseResult =>
            new TransferInput(
                Namespace: parseResult.GetValueForOption(ServiceBusToolCommand.NamespaceOption),
                SourcePath: parseResult.GetValueForArgument(SourcePathArgument),
                TargetQueueOrTopic: parseResult.GetValueForArgument(TargetQueueOrTopic)));
}