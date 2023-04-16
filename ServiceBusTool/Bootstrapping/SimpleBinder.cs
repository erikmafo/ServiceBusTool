using System.CommandLine.Binding;

namespace ServiceBusTool.Bootstrapping;

public class SimpleBinder<TService> : BinderBase<TService>
{
    private readonly Func<BindingContext, TService> _provideService;

    public SimpleBinder(Func<BindingContext, TService> provideService)
    {
        _provideService = provideService;
    }

    protected override TService GetBoundValue(BindingContext bindingContext) => _provideService(bindingContext);
}