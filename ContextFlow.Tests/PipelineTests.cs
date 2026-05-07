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
            new DelegateStep(2, testContext =>
            {
                testContext.ExecutedSteps.Add("second");
                return true;
            }),
            new DelegateStep(1, testContext =>
            {
                testContext.ExecutedSteps.Add("first");
                return true;
            }),
            new DelegateStep(3, testContext =>
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
            new DelegateStep(1, testContext =>
            {
                testContext.ExecutedSteps.Add("first");
                return true;
            }),
            new DelegateStep(2, testContext =>
            {
                testContext.ExecutedSteps.Add("stop");
                return false;
            }),
            new DelegateStep(3, testContext =>
            {
                testContext.ExecutedSteps.Add("third");
                return true;
            })
        };

        var sut = new Pipeline<TestContext>(steps);
        await sut.ExecuteAsync(context);

        Assert.Equal(["first", "stop"], context.ExecutedSteps);
    }

    private sealed class TestContext : IContext
    {
        public List<string> ExecutedSteps { get; } = [];
    }

    private sealed class DelegateStep(int order, Func<TestContext, bool> callback) : IPipelineStep<TestContext>
    {
        public int Order { get; } = order;

        public Task<bool> ExecuteAsync(TestContext context, CancellationToken ct = default)
        {
            return Task.FromResult(callback(context));
        }
    }

    
}
