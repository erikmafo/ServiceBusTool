using System.CommandLine.Binding;

namespace ServiceBusTool.Bootstrapping;

public interface IServiceBinderFactory
{
    BinderBase<T> CreateBinder<T>();
}