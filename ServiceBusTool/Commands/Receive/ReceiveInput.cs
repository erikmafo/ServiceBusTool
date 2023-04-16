namespace ServiceBusTool.Commands.Receive;

public record ReceiveInput(
    string Namespace, 
    string EntityPath,
    string ReceiveMode,
    TimeSpan? MaxWaitTime,
    int? MaxMessages,
    string Encoding,
    bool Acknowledge,
    bool IncludeProperties);