using System.CommandLine;
using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Purge;

public class PurgeHandler : GenericHandlerBase<PurgeInput>
{
    private readonly IServiceBusHelper _serviceBus;

    public PurgeHandler(IServiceBusHelper serviceBus)
    {
        _serviceBus = serviceBus;
    }

    public override async Task Handle(PurgeInput input, CancellationToken token = default)
    {
        using var reporter = new ProcessMessagesReporter();
        var sourcePath = EntityPath.Parse(input.EntityPath);
        Console.WriteLine($"Purging messages from {input.EntityPath}");
        if (!UserWishToContinue())
        {
            return;
        }
        
        if (sourcePath.IsQueue)
        {
            await _serviceBus.PurgeMessagesAsync(
                input.Namespace, 
                sourcePath.Queue,
                sourcePath.SubQueue,
                reporter,
                token);
        }
        else
        {
            await _serviceBus.PurgeMessagesAsync(
                input.Namespace, 
                sourcePath.Topic,
                sourcePath.Subscription,
                sourcePath.SubQueue,
                reporter,
                token);
        }
    }
}