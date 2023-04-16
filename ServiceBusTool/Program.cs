using System.CommandLine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusTool.Bootstrapping;
using ServiceBusTool.Commands;
using ServiceBusTool.Commands.Describe.Queue;
using ServiceBusTool.Commands.Describe.Subscription;
using ServiceBusTool.Commands.Describe.Topic;
using ServiceBusTool.Commands.Purge;
using ServiceBusTool.Commands.Receive;
using ServiceBusTool.Commands.Transfer;
using ServiceBusTool.ServiceBus;

return await new CommandLineApp<ServiceBusToolCommand>()
    .ConfigureAppConfiguration(builder => builder
        .AddJsonFile(ApplicationData.JsonConfigPath, optional: true)
        .AddEnvironmentVariables())
    .ConfigureServices((services, _) => services
        .AddSingleton<IServiceBusHelper, ServiceBusHelper>()
        .AddCommandHandler<DescribeTopicInput, DescribeTopicHandler>()
        .AddCommandHandler<DescribeSubscriptionInput, DescribeSubscriptionHandler>()
        .AddCommandHandler<DescribeQueueInput, DescribeQueueHandler>()
        .AddCommandHandler<TransferInput, TransferHandler>()
        .AddCommandHandler<ReceiveInput, ReceiveHandler>()
        .AddCommandHandler<PurgeInput, PurgeHandler>())
    .ConfigureCommandLine(builder => builder.UseDefaults())
    .InvokeAsync(args);