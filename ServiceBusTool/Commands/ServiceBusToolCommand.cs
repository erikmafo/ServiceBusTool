using System.CommandLine;
using ServiceBusTool.Commands.Describe;
using ServiceBusTool.Commands.Purge;
using ServiceBusTool.Commands.Receive;
using ServiceBusTool.Commands.Transfer;

namespace ServiceBusTool.Commands;

public class ServiceBusToolCommand : RootCommand
{
    public static readonly Option<string> NamespaceOption = new(
        aliases: new[] { "--namespace", "-ns" },
        description: "Name of the service bus namespace.");

    public ServiceBusToolCommand() : 
        base("CLI for interacting with service bus")
    {
        AddGlobalOption(NamespaceOption);
        
        AddCommand(Describe);
        AddCommand(Purge);
        AddCommand(Transfer);
        AddCommand(Receive);
    }

    public DescribeCommand Describe { get; } = new();

    public PurgeCommand Purge { get; } = new();

    public TransferCommand Transfer { get; } = new();

    public ReceiveCommand Receive { get; } = new();
}