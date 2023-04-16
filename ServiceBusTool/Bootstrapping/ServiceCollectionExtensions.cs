using Microsoft.Extensions.DependencyInjection;
using ServiceBusTool.Commands.Base;

namespace ServiceBusTool.Bootstrapping;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandler<TInput, THandler>(this IServiceCollection services) 
        where THandler : class, IGenericHandler<TInput> =>
        services.AddSingleton<IGenericHandler<TInput>, THandler>();
}