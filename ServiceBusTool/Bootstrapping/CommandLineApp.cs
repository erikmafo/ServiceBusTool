using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServiceBusTool.Commands.Base;

namespace ServiceBusTool.Bootstrapping;

public class CommandLineApp<TRootCommand> where TRootCommand : RootCommand
{
    private readonly IConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();
    private readonly List<Action<IServiceCollection, IConfiguration>> _configureServicesActions = new();
    private readonly List<Action<CommandLineBuilder>> _commandLineBuilderActions = new();

    public CommandLineApp<TRootCommand> ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
    {
        configure(_configurationBuilder);
        return this;
    }
    
    public CommandLineApp<TRootCommand> ConfigureServices(Action<IServiceCollection, IConfiguration> configure)
    {
        _configureServicesActions.Add(configure);
        return this;
    }

    public CommandLineApp<TRootCommand> ConfigureCommandLine(Action<CommandLineBuilder> configure)
    {
        _commandLineBuilderActions.Add(configure);
        return this;
    }

    public async Task<int> InvokeAsync(string[] args)
    {
        var configuration = BuildConfiguration();
        var serviceProvider = BuildServiceProvider(configuration);
        var rootCommand = BuildRootCommand(serviceProvider);
        ConfigureHandlers(rootCommand, serviceProvider.GetRequiredService<IServiceBinderFactory>());
        var commandLine = BuildCommandLine(rootCommand);
        
        return await commandLine.InvokeAsync(args);
    }

    private IConfiguration BuildConfiguration() => _configurationBuilder.Build();

    private TRootCommand BuildRootCommand(IServiceProvider serviceProvider) => 
        serviceProvider.GetRequiredService<TRootCommand>();

    private Parser BuildCommandLine(TRootCommand rootCommand)
    {
        var commandLineBuilder = new CommandLineBuilder(rootCommand);
        _commandLineBuilderActions.ForEach(action => action(commandLineBuilder));
        return commandLineBuilder.Build();
    }

    private ServiceProvider BuildServiceProvider(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.TryAddSingleton<IServiceBinderFactory, ServiceBinderFactory>();
        serviceCollection.TryAddSingleton<TRootCommand>();
        _configureServicesActions.ForEach(action => action(serviceCollection, configuration));
        
        return serviceCollection.BuildServiceProvider();
    }

    private void ConfigureHandlers(Command command, IServiceBinderFactory binderFactory)
    {
        if (command is InputBoundCommand inputBoundCommand)
        {
            inputBoundCommand.ConfigureHandler(binderFactory);
        }

        foreach (var subcommand in command.Subcommands)
        {
            ConfigureHandlers(subcommand, binderFactory);
        }
    }
}