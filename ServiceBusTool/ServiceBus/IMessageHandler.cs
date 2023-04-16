using Azure.Messaging.ServiceBus;

namespace ServiceBusTool.ServiceBus;

public interface IMessageHandler
{
    void ReceiveMessage(ServiceBusReceivedMessage message);
}