using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Receive;

public class ReceiveHandler : GenericHandlerBase<ReceiveInput>
{
    private readonly IServiceBusHelper _serviceBus;

    public ReceiveHandler(IServiceBusHelper serviceBus)
    {
        _serviceBus = serviceBus;
    }

    public override async Task Handle(ReceiveInput input, CancellationToken token = default)
    {
        var receiveOptions = new ReceiveOptions
        {
            ReceiveMode = input.ReceiveMode,
            MaxMessages = input.MaxMessages,
            MaxWaitTime = input.MaxWaitTime,
            Acknowledge = input.Acknowledge
        };
        
        var receiver = new MessageHandler(Console, input.Encoding);
        var sourcePath = EntityPath.Parse(input.EntityPath);
        if (sourcePath.IsQueue)
        {
            await _serviceBus.ReceiveMessagesAsync(
                input.Namespace, 
                sourcePath.Queue,
                sourcePath.SubQueue,
                receiver,
                receiveOptions,
                token);
        }
        else
        {
            await _serviceBus.ReceiveMessagesAsync(
                input.Namespace, 
                sourcePath.Topic,
                sourcePath.Subscription,
                sourcePath.SubQueue,
                receiver,
                receiveOptions,
                token);
        }
    }
}