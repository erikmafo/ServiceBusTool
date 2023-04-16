using System.CommandLine;
using System.CommandLine.Binding;
using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Purge;

public class PurgeCommand : GenericCommand<PurgeInput>
{
    public static readonly Argument<string> EntityPathArgument = 
        new() {
            Name = "EntityPath",
            Description = 
                @"The queue or subscription to purge messages from. Valid formats:
    - {queue}
    - {queue}/${subQueue} 
    - {topic}/Subscriptions/{subscription}
    - {topic}/Subscriptions/{subscription}
    - {topic}/Subscriptions/{subscription}/${subQueue}" };
    
    public PurgeCommand() : base(
        name: "purge",
        description: "Purge messages from a queue or subscription.")
    {
        AddArgument(EntityPathArgument);
        AddValidator(commandResult =>
        {
            var entityPath = commandResult.GetValueForArgument(EntityPathArgument);
            if (!EntityPath.IsValid(entityPath, out var errorMessage))
            {
                commandResult.ErrorMessage = errorMessage;
            }
        });
    }

    public override BinderBase<PurgeInput> InputBinder => 
        CreateInputBinder(parseResult =>
            new PurgeInput(
                Namespace: parseResult.GetValueForOption(ServiceBusToolCommand.NamespaceOption),
                EntityPath: parseResult.GetValueForArgument(EntityPathArgument)));
}