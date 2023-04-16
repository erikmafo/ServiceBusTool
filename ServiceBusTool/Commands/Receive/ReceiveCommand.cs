using System.CommandLine;
using System.CommandLine.Binding;
using ServiceBusTool.Commands.Base;

namespace ServiceBusTool.Commands.Receive;

public class ReceiveCommand : GenericCommand<ReceiveInput>
{
    private static readonly Argument<string> EntityPathArgument = 
        new() 
        { 
            Name = "EntityPath",
            Description = 
                @"Path to the entity, (queue or subscription), to receive messages from. Valid formats:
    - {queue}
    - {queue}/${subQueue} 
    - {topic}/Subscriptions/{subscription}
    - {topic}/Subscriptions/{subscription}
    - {topic}/Subscriptions/{subscription}/${subQueue}" 
        };

    private static readonly Option<string> ReceiveModeOption = new(
        aliases: new[] { "--receive-mode" },
        description: "Specify receive mode PeekLock or ReceiveAndDelete",
        getDefaultValue: () => "PeekLock");

    private static readonly Option<TimeSpan?> MaxWaitTimeOption = new(
        aliases: new[] { "--max-wait-time" },
        description: "The max time to wait for a message");
    
    private static readonly Option<int?> MaxMessagesOption = new(
        aliases: new[] { "--max-messages" },
        description: "The max number of messages to receive");

    private static readonly Option<string> EncodingOption = new(
        aliases: new[] { "--encoding" },
        description: "Specify how the body of the message should be encoded in the output",
        getDefaultValue: () => "ut8");

    private static readonly Option<bool> AcknowledgeOption = new(
        aliases: new[] { "--acknowledge", "-ack" },
        description: "Deletes messages from the queue/subscription after being written to the console.",
        getDefaultValue: () => false);

    private static readonly Option<bool> IncludePropertiesOption = new(
        aliases: new[] { "include-properties" },
        description: "Include properties for each message.", 
        getDefaultValue: () => false);

    public ReceiveCommand() : base(
        name: "receive", 
        description: "Receive messages from a queue or subscription")
    {
        AddArgument(EntityPathArgument);
        AddOption(ReceiveModeOption);
        AddOption(MaxWaitTimeOption);
        AddOption(MaxMessagesOption);
        AddOption(EncodingOption);
        AddOption(AcknowledgeOption);
    }

    public override BinderBase<ReceiveInput> InputBinder => 
        CreateInputBinder(parseResult =>
            new ReceiveInput(
                Namespace: parseResult.GetValueForOption(ServiceBusToolCommand.NamespaceOption),
                EntityPath: parseResult.GetValueForArgument(EntityPathArgument),
                ReceiveMode: parseResult.GetValueForOption(ReceiveModeOption),
                MaxWaitTime: parseResult.GetValueForOption(MaxWaitTimeOption),
                MaxMessages: parseResult.GetValueForOption(MaxMessagesOption),
                Encoding: parseResult.GetValueForOption(EncodingOption),
                Acknowledge: parseResult.GetValueForOption(AcknowledgeOption),
                IncludeProperties: parseResult.GetValueForOption(IncludePropertiesOption)));
}