using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using ServiceBusTool.Bootstrapping;

namespace ServiceBusTool.Commands.Base;

public abstract class GenericCommand<TInput> : InputBoundCommand
{
    protected GenericCommand(
        string name, 
        string description = null) 
        : base(name, description)
    {
    }

    public abstract BinderBase<TInput> InputBinder { get; }

    public override void ConfigureHandler(IServiceBinderFactory binderFactory) => 
        SetGenericHandler(binderFactory.CreateBinder<IGenericHandler<TInput>>());

    protected BinderBase<TInput> CreateInputBinder(Func<ParseResult, TInput> extractInput) => 
        new SimpleBinder<TInput>(context => extractInput(context.ParseResult));
    
    private void SetGenericHandler<THandler>(
        IValueDescriptor<THandler> handlerBinder) 
        where THandler : IGenericHandler<TInput> =>
        this.SetHandler(
            async (handler, input, console, token) =>
            {
                handler.Console = console;
                await handler.Handle(input, token);
            },
            handlerBinder,
            InputBinder,
            ConsoleBinder,
            CancellationTokenBinder);

    private static SimpleBinder<CancellationToken> CancellationTokenBinder =>
        new(context => context.GetRequiredService<InvocationContext>().GetCancellationToken());

    private static SimpleBinder<IConsole> ConsoleBinder =>
        new(context => context.GetRequiredService<InvocationContext>().Console);
}