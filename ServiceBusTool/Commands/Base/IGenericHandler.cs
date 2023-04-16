using System.CommandLine;

namespace ServiceBusTool.Commands.Base;

public interface IGenericHandler<in TInput>
{
    IConsole Console { get; set; }
    
    Task Handle(TInput input, CancellationToken token = default);
}