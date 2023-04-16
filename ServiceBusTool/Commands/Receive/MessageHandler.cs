using System.CommandLine;
using Azure.Messaging.ServiceBus;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Receive;

public class MessageHandler : IMessageHandler
{
    private readonly IConsole _console;
    private readonly string _encoding;

    public MessageHandler(IConsole console, string encoding)
    {
        _console = console;
        _encoding = encoding;
    }

    public void ReceiveMessage(ServiceBusReceivedMessage message)
    {
        var bytes = message.Body.ToArray();

        var body = _encoding switch
        {
            "hex" => Convert.ToHexString(bytes),
            "ut8" => System.Text.Encoding.UTF8.GetString(bytes),
            _ => throw new NotSupportedException($"{_encoding} encoding not supported")
        };
        
        _console.WriteLine(body);
    }
}