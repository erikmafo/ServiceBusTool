using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBusTool.Bootstrapping;

public class ServiceBinderFactory : IServiceBinderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceBinderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public BinderBase<T> CreateBinder<T>() => new SimpleBinder<T>(_ => _serviceProvider.GetRequiredService<T>());
}