using Microsoft.Extensions.DependencyInjection;

namespace ContextFlow.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddContextFlows_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddContextFlows([typeof(ServiceCollectionExtensionsTests).Assembly]);

        Assert.Same(services, result);
    }

    [Fact]
    public void AddContextFlows_RegistersPipelineAndSteps_ForDiscoveredContext()
    {
        var services = new ServiceCollection();
        services.AddContextFlows([typeof(ServiceCollectionExtensionsTests).Assembly]);
        var provider = services.BuildServiceProvider();

        var pipeline = provider.GetService<IPipeline<PipelineContext>>();
        var steps = provider.GetServices<IPipelineStep<PipelineContext>>();

        Assert.NotNull(pipeline);
        Assert.Contains(steps, step => step.GetType() == typeof(PipelineContextStep));
    }

    [Fact]
    public void AddContextFlows_RegistersChainAndSteps_ForDiscoveredContext()
    {
        var services = new ServiceCollection();
        services.AddContextFlows([typeof(ServiceCollectionExtensionsTests).Assembly]);
        var provider = services.BuildServiceProvider();

        var chain = provider.GetService<IChainOfResponsibility<ChainContext>>();
        var steps = provider.GetServices<IChainOfResponsibilityStep<ChainContext>>();

        Assert.NotNull(chain);
        Assert.Contains(steps, step => step.GetType() == typeof(ChainContextStep));
    }

    [Fact]
    public void AddContextFlows_RegistersDependencyGraphAndSteps_ForDiscoveredContext()
    {
        var services = new ServiceCollection();
        services.AddContextFlows([typeof(ServiceCollectionExtensionsTests).Assembly]);
        var provider = services.BuildServiceProvider();

        var graph = provider.GetService<IDependencyGraph<DependencyContext>>();
        var steps = provider.GetServices<IDependencyGraphStep<DependencyContext>>();

        Assert.NotNull(graph);
        Assert.Contains(steps, step => step.GetType() == typeof(DependencyContextStep));
    }

    [Fact]
    public void AddContextFlows_DoesNotRegisterExecutionServices_WhenContextHasNoSteps()
    {
        var services = new ServiceCollection();
        services.AddContextFlows([typeof(ServiceCollectionExtensionsTests).Assembly]);
        var provider = services.BuildServiceProvider();

        Assert.Null(provider.GetService<IPipeline<NoStepContext>>());
        Assert.Null(provider.GetService<IChainOfResponsibility<NoStepContext>>());
        Assert.Null(provider.GetService<IDependencyGraph<NoStepContext>>());
    }

    private sealed class PipelineContext : IContext;

    private sealed class PipelineContextStep : IPipelineStep<PipelineContext>
    {
        public int Order => 1;

        public Task<bool> ExecuteAsync(PipelineContext context, CancellationToken? ct = default)
        {
            return Task.FromResult(true);
        }
    }

    private sealed class ChainContext : IContext;

    private sealed class ChainContextStep : IChainOfResponsibilityStep<ChainContext>
    {
        public int Order => 1;

        public Task ExecuteAsync(ChainContext context, Func<ChainContext, CancellationToken?, Task> next, CancellationToken? ct = default)
        {
            return next(context, ct);
        }
    }

    private sealed class DependencyContext : IContext;

    private sealed class DependencyContextStep : IDependencyGraphStep<DependencyContext>
    {
        public Type[] PreviousTypes => [];

        public Task<bool> ExecuteAsync(DependencyContext context, CancellationToken? ct = default)
        {
            return Task.FromResult(true);
        }
    }

    private sealed class NoStepContext : IContext;
}
