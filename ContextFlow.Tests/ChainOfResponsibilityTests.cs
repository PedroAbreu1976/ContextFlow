namespace ContextFlow.Tests;

public class ChainOfResponsibilityTests
{
    [Fact]
    public async Task ExecuteAsync_Throws_WhenNoStepsFound()
    {
        var sut = new ChainOfResponsibility<TestContext>([]);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(new TestContext()));

        Assert.Contains("No steps found for context of type TestContext", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesStepsInOrder_AndReturnsContext()
    {
        var context = new TestContext();
        var steps = new IChainOfResponsibilityStep<TestContext>[]
        {
            new DelegateStep(2, async (testContext, next, ct) =>
            {
                testContext.ExecutedSteps.Add("second");
                await next(testContext, ct);
            }),
            new DelegateStep(1, async (testContext, next, ct) =>
            {
                testContext.ExecutedSteps.Add("first");
                await next(testContext, ct);
            }),
            new DelegateStep(3, async (testContext, next, ct) =>
            {
                testContext.ExecutedSteps.Add("third");
                await next(testContext, ct);
            })
        };

        var sut = new ChainOfResponsibility<TestContext>(steps);
        var result = await sut.ExecuteAsync(context);

        Assert.Same(context, result);
        Assert.Equal(["first", "second", "third"], context.ExecutedSteps);
    }

    [Fact]
    public async Task ExecuteAsync_Stops_WhenStepDoesNotCallNext()
    {
        var context = new TestContext();
        var steps = new IChainOfResponsibilityStep<TestContext>[]
        {
            new DelegateStep(1, async (testContext, next, ct) =>
            {
                testContext.ExecutedSteps.Add("first");
                await next(testContext, ct);
            }),
            new DelegateStep(2, (testContext, _, _) =>
            {
                testContext.ExecutedSteps.Add("stop");
                return Task.CompletedTask;
            }),
            new DelegateStep(3, async (testContext, next, ct) =>
            {
                testContext.ExecutedSteps.Add("third");
                await next(testContext, ct);
            })
        };

        var sut = new ChainOfResponsibility<TestContext>(steps);
        await sut.ExecuteAsync(context);

        Assert.Equal(["first", "stop"], context.ExecutedSteps);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationTokenToSteps()
    {
        var context = new TestContext();
        var cts = new CancellationTokenSource();
        CancellationToken? tokenFromFirstStep = null;
        CancellationToken? tokenFromSecondStep = null;

        var steps = new IChainOfResponsibilityStep<TestContext>[]
        {
            new DelegateStep(1, async (testContext, next, ct) =>
            {
                tokenFromFirstStep = ct;
                await next(testContext, ct);
            }),
            new DelegateStep(2, (testContext, _, ct) =>
            {
                tokenFromSecondStep = ct;
                return Task.CompletedTask;
            })
        };

        var sut = new ChainOfResponsibility<TestContext>(steps);
        await sut.ExecuteAsync(context, cts.Token);

        Assert.Equal(cts.Token, tokenFromFirstStep);
        Assert.Equal(cts.Token, tokenFromSecondStep);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotExecuteSteps_WhenCancellationAlreadyRequested()
    {
        var context = new TestContext();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var steps = new IChainOfResponsibilityStep<TestContext>[]
        {
            new DelegateStep(1, async (testContext, next, ct) =>
            {
                testContext.ExecutedSteps.Add("first");
                await next(testContext, ct);
            }),
            new DelegateStep(2, (testContext, _, _) =>
            {
                testContext.ExecutedSteps.Add("second");
                return Task.CompletedTask;
            })
        };

        var sut = new ChainOfResponsibility<TestContext>(steps);
        var result = await sut.ExecuteAsync(context, cts.Token);

        Assert.Same(context, result);
        Assert.Empty(context.ExecutedSteps);
    }

    [Fact]
    public async Task ExecuteAsync_PropagatesCancellation_WhenTokenCanceledDuringExecution()
    {
        var context = new TestContext();
        var cts = new CancellationTokenSource();

        var steps = new IChainOfResponsibilityStep<TestContext>[]
        {
            new DelegateStep(1, async (testContext, next, ct) =>
            {
                testContext.ExecutedSteps.Add("first");
                cts.Cancel();
                await next(testContext, ct);
            }),
            new DelegateStep(2, (testContext, _, ct) =>
            {
                ct?.ThrowIfCancellationRequested();
                testContext.ExecutedSteps.Add("second");
                return Task.CompletedTask;
            })
        };

        var sut = new ChainOfResponsibility<TestContext>(steps);

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.ExecuteAsync(context, cts.Token));
        Assert.Equal(["first"], context.ExecutedSteps);
    }

    private sealed class TestContext : IContext
    {
        public List<string> ExecutedSteps { get; } = [];
    }

    private sealed class DelegateStep(
        int order,
        Func<TestContext, Func<TestContext, CancellationToken?, Task>, CancellationToken?, Task> callback)
        : IChainOfResponsibilityStep<TestContext>
    {
        public int Order { get; } = order;

        public Task ExecuteAsync(TestContext context, Func<TestContext, CancellationToken?, Task> next, CancellationToken? ct = default)
        {
            return callback(context, next, ct);
        }
    }
}
