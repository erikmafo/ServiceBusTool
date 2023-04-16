using ServiceBusTool.Commands.Base;
using ServiceBusTool.ServiceBus;

namespace ServiceBusTool.Commands.Describe.Queue;

public class DescribeQueueHandler : GenericHandlerBase<DescribeQueueInput>
{
    private readonly IServiceBusHelper _serviceBus;

    public DescribeQueueHandler(IServiceBusHelper serviceBus)
    {
        _serviceBus = serviceBus;
    }

    public override async Task Handle(DescribeQueueInput input, CancellationToken token = default)
    {
        await HandleQueueProperties(input, token);
        await HandleQueueRuntimeProperties(input, token);
    }

    private async Task HandleQueueProperties(DescribeQueueInput input, CancellationToken token)
    {
        var properties = await _serviceBus.GetQueuePropertiesAsync(input.Namespace, input.Queue, token);
        Output(properties);
    }

    private async Task HandleQueueRuntimeProperties(DescribeQueueInput input, CancellationToken token)
    {
        var runtimeProperties = await _serviceBus.GetQueueRuntimePropertiesAsync(input.Namespace, input.Queue, token);
        Output(runtimeProperties);
    }
}