namespace ServiceBusTool.ServiceBus;

public record ReceiveOptions
{
    public string ReceiveMode { init; get; } = "PeekLock";
    
    public int? MaxMessages { init; get; } = null;

    public TimeSpan? MaxWaitTime { init; get; } = null;
    
    public bool Acknowledge { init; get; }
}