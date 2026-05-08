namespace ContextFlow.Tests;

public class PipelineTests
{
    [Fact]
    public async Task ExecuteAsync_Throws_WhenNoStepsFound()
    {
        var sut = new Pipeline<TestContext>([]);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(new TestContext()));

        Assert.Contains("No steps found for context of type TestContext", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesStepsInOrder_AndReturnsContext()
    {
        var context = new TestContext();
        var steps = new IPipelineStep<TestContext>[]
        {
            new DelegateStep(2, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("second");
                return true;
            }),
            new DelegateStep(1, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("first");
                return true;
            }),
            new DelegateStep(3, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("third");
                return true;
            })
        };

        var sut = new Pipeline<TestContext>(steps);
        var result = await sut.ExecuteAsync(context);

        Assert.Same(context, result);
        Assert.Equal(["first", "second", "third"], context.ExecutedSteps);
    }

    [Fact]
    public async Task ExecuteAsync_Stops_WhenStepReturnsFalse()
    {
        var context = new TestContext();
        var steps = new IPipelineStep<TestContext>[]
        {
            new DelegateStep(1, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("first");
                return true;
            }),
            new DelegateStep(2, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("stop");
                return false;
            }),
            new DelegateStep(3, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("third");
                return true;
            })
        };

        var sut = new Pipeline<TestContext>(steps);
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

        var steps = new IPipelineStep<TestContext>[]
        {
            new DelegateStep(1, (_, ct) =>
            {
                tokenFromFirstStep = ct;
                return true;
            }),
            new DelegateStep(2, (_, ct) =>
            {
                tokenFromSecondStep = ct;
                return true;
            })
        };

        var sut = new Pipeline<TestContext>(steps);
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

        var steps = new IPipelineStep<TestContext>[]
        {
            new DelegateStep(1, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("first");
                return true;
            }),
            new DelegateStep(2, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("second");
                return true;
            })
        };

        var sut = new Pipeline<TestContext>(steps);
        var result = await sut.ExecuteAsync(context, cts.Token);

        Assert.Same(context, result);
        Assert.Empty(context.ExecutedSteps);
    }

    [Fact]
    public async Task ExecuteAsync_StopsExecutingRemainingSteps_WhenCancellationRequestedDuringExecution()
    {
        var context = new TestContext();
        var cts = new CancellationTokenSource();

        var steps = new IPipelineStep<TestContext>[]
        {
            new DelegateStep(1, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("first");
                cts.Cancel();
                return true;
            }),
            new DelegateStep(2, (testContext, _) =>
            {
                testContext.ExecutedSteps.Add("second");
                return true;
            })
        };

        var sut = new Pipeline<TestContext>(steps);
        await sut.ExecuteAsync(context, cts.Token);

        Assert.Equal(["first"], context.ExecutedSteps);
    }

    private sealed class TestContext : IContext
    {
        public List<string> ExecutedSteps { get; } = [];
    }

    private sealed class DelegateStep(int order, Func<TestContext, CancellationToken?, bool> callback) : IPipelineStep<TestContext>
    {
        public int Order { get; } = order;

        public Task<bool> ExecuteAsync(TestContext context, CancellationToken? ct = default)
        {
            return Task.FromResult(callback(context, ct));
        }
    }

    
}
