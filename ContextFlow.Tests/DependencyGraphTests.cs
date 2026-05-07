namespace ContextFlow.Tests;

public class DependencyGraphTests
{
    [Fact]
    public async Task ExecuteAsync_Throws_WhenNoStepsFound()
    {
        var sut = new DependencyGraph<TestContext>([]);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(new TestContext()));

        Assert.Contains("No steps found for context of type TestContext", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesStepsByDependencies_AndReturnsContext()
    {
        var context = new TestContext();
        var steps = new IDependencyGraphStep<TestContext>[]
        {
            new StepA((testContext, _) =>
            {
                testContext.ExecutedSteps.Add("A");
                return true;
            }),
            new StepB((testContext, _) =>
            {
                testContext.ExecutedSteps.Add("B");
                return true;
            }),
            new StepC((testContext, _) =>
            {
                testContext.ExecutedSteps.Add("C");
                return true;
            })
        };

        var sut = new DependencyGraph<TestContext>(steps);
        var result = await sut.ExecuteAsync(context);

        Assert.Same(context, result);
        Assert.Equal(["A", "B", "C"], context.ExecutedSteps);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotExecuteDependentStep_WhenParentReturnsFalse()
    {
        var context = new TestContext();
        var steps = new IDependencyGraphStep<TestContext>[]
        {
            new StepA((testContext, _) =>
            {
                testContext.ExecutedSteps.Add("A");
                return false;
            }),
            new StepB((testContext, _) =>
            {
                testContext.ExecutedSteps.Add("B");
                return true;
            })
        };

        var sut = new DependencyGraph<TestContext>(steps);
        await sut.ExecuteAsync(context);

        Assert.Equal(["A"], context.ExecutedSteps);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationTokenToSteps()
    {
        var context = new TestContext();
        var cts = new CancellationTokenSource();
        CancellationToken? tokenFromStepA = null;
        CancellationToken? tokenFromStepB = null;

        var steps = new IDependencyGraphStep<TestContext>[]
        {
            new StepA((_, ct) =>
            {
                tokenFromStepA = ct;
                return true;
            }),
            new StepB((_, ct) =>
            {
                tokenFromStepB = ct;
                return true;
            })
        };

        var sut = new DependencyGraph<TestContext>(steps);
        await sut.ExecuteAsync(context, cts.Token);

        Assert.Equal(cts.Token, tokenFromStepA);
        Assert.Equal(cts.Token, tokenFromStepB);
    }

    private sealed class TestContext : IContext
    {
        public List<string> ExecutedSteps { get; } = [];
    }

    private sealed class StepA(Func<TestContext, CancellationToken?, bool> callback) : IDependencyGraphStep<TestContext>
    {
        public Type[] PreviousTypes { get; } = [];

        public Task<bool> ExecuteAsync(TestContext context, CancellationToken? ct = default)
        {
            return Task.FromResult(callback(context, ct));
        }
    }

    private sealed class StepB(Func<TestContext, CancellationToken?, bool> callback) : IDependencyGraphStep<TestContext>
    {
        public Type[] PreviousTypes { get; } = [typeof(StepA)];

        public Task<bool> ExecuteAsync(TestContext context, CancellationToken? ct = default)
        {
            return Task.FromResult(callback(context, ct));
        }
    }

    private sealed class StepC(Func<TestContext, CancellationToken?, bool> callback) : IDependencyGraphStep<TestContext>
    {
        public Type[] PreviousTypes { get; } = [typeof(StepA), typeof(StepB)];

        public Task<bool> ExecuteAsync(TestContext context, CancellationToken? ct = default)
        {
            return Task.FromResult(callback(context, ct));
        }
    }
}
