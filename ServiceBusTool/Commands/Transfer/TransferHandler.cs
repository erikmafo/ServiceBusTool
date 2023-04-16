using System.CommandLine;
using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Transfer;

public class TransferHandler : GenericHandlerBase<TransferInput>
{
    private readonly IServiceBusHelper _serviceBus;

    public TransferHandler(IServiceBusHelper serviceBus)
    {
        _serviceBus = serviceBus;
    }

    public override async Task Handle(TransferInput input, CancellationToken token = default)
    {
        using var reporter = new ProcessMessagesReporter();
        var sourcePath = EntityPath.Parse(input.SourcePath);
        Console.WriteLine($"Transferring messages from {input.SourcePath} to {input.TargetQueueOrTopic}");
        if (!UserWishToContinue())
        {
            return;
        }
        
        if (sourcePath.IsQueue)
        {
            await _serviceBus.TransferMessagesAsync(
                input.Namespace, 
                sourcePath.Queue,
                sourcePath.SubQueue,
                targetEntity: input.TargetQueueOrTopic,
                reporter,
                token);
        }
        else
        {
            await _serviceBus.TransferMessagesAsync(
                input.Namespace, 
                sourcePath.Topic,
                sourcePath.Subscription,
                sourcePath.SubQueue,
                targetEntity: input.TargetQueueOrTopic,
                reporter,
                token);
        }
    }
}