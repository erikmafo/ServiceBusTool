using System.CommandLine;

namespace ServiceBusTool.Commands.Base;

public abstract class GenericHandlerBase<TInput> : IGenericHandler<TInput>
{
    public IConsole Console { get; set; }
    
    public abstract Task Handle(TInput input, CancellationToken token = default);

    protected void Output(object obj)
    {
        foreach (var property in obj.GetType().GetProperties())
        {
            OutputNamedValue(property.Name, property.GetValue(obj));
        }
    }

    protected void OutputNamedValue(string name, object value) => 
        Console.WriteLine($"{name}: \t {value}");

    protected virtual bool UserWishToContinue()
    {
        Console.WriteLine("Do you wish to continue? (Y)es (N)o");
        var yesNo = System.Console.ReadKey(true);
        return yesNo.Key == ConsoleKey.Y;
    }
}