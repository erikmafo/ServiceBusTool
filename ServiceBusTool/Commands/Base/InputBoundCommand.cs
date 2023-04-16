using System.CommandLine;
using ServiceBusTool.Bootstrapping;

namespace ServiceBusTool.Commands.Base;

public abstract class InputBoundCommand : Command
{
    protected InputBoundCommand(
        string name, 
        string description = null) 
        : base(name, description)
    {
    }

    public abstract void ConfigureHandler(IServiceBinderFactory binderFactory);
}