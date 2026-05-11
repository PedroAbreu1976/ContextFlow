using ContextFlow.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ContextFlow;

/// <summary>
/// Provides extension methods for registering ContextFlow services in a dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers pipeline-related services discovered in the provided assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">Assemblies to scan for contexts and steps.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPipelines(
        this IServiceCollection services, 
        Assembly[] assemblies, 
        Action<ContextFlowConfiguration>? configuration =  default)
    {
        services.ConfigureOptions<ContextFlowConfigurationSetup>();
        services.PostConfigure<ContextFlowConfiguration>(configuration ?? (opt => { }));

        services.AddTransient<ContextFlowLogger>();

        var typesMetadata = assemblies?
            .SelectMany(x => x.GetContextTypes())
            .ToDictionary(
                x => x,
                x => new
                {
                    PipelineSteps = assemblies?
                        .SelectMany(s => s.GetStepTypesForContext(x, typeof(IPipelineStep<>))) ?? [],
                    ChainOfResponsibilitySteps = assemblies?
                        .SelectMany(a => a.GetStepTypesForContext(x, typeof(IChainOfResponsibilityStep<>))) ?? [],
                    DependencyTreeSteps = assemblies?
                        .SelectMany(a => a.GetStepTypesForContext(x, typeof(IDependencyGraphStep<>))) ?? []
                });

        foreach (var context in typesMetadata ?? [])
        {
            if (context.Value.PipelineSteps.Any())
            {
                services.AddTransient(typeof(IPipeline<>).MakeGenericType(context.Key), typeof(Pipeline<>).MakeGenericType(context.Key));
                foreach (var step in context.Value.PipelineSteps)
                {
                    services.AddTransient(typeof(IPipelineStep<>).MakeGenericType(context.Key), step);
                }
            }
            if (context.Value.ChainOfResponsibilitySteps.Any())
            {
                services.AddTransient(typeof(IChainOfResponsibility<>).MakeGenericType(context.Key), typeof(ChainOfResponsibility<>).MakeGenericType(context.Key));
                foreach (var step in context.Value.ChainOfResponsibilitySteps)
                {
                    services.AddTransient(typeof(IChainOfResponsibilityStep<>).MakeGenericType(context.Key), step);
                }
            }
            if (context.Value.DependencyTreeSteps.Any())
            {
                services.AddTransient(typeof(IDependencyGraph<>).MakeGenericType(context.Key), typeof(DependencyGraph<>).MakeGenericType(context.Key));
                foreach (var step in context.Value.DependencyTreeSteps)
                {
                    services.AddTransient(typeof(IDependencyGraphStep<>).MakeGenericType(context.Key), step);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Gets all concrete context types from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>A sequence of discovered context types.</returns>
    private static IEnumerable<Type> GetContextTypes(this Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IContext).IsAssignableFrom(t));
    }

    /// <summary>
    /// Gets all concrete step types for a context and step interface from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <param name="contextType">The context type used for generic matching.</param>
    /// <param name="stepType">The open generic step interface type.</param>
    /// <returns>A sequence of discovered step implementation types.</returns>
    private static IEnumerable<Type> GetStepTypesForContext(this Assembly assembly, Type contextType, Type stepType)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => stepType.MakeGenericType(contextType).IsAssignableFrom(t));
    }
}
