using System.CommandLine.Binding;

namespace ServiceBusTool.Bootstrapping;

public static class BindingContextExtensions
{
    public static T GetRequiredService<T>(this BindingContext context) => (T)context.GetService(typeof(T))!;
}