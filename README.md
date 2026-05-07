# ContextFlow

ContextFlow is a lightweight .NET library that simplifies the implementation of common design patterns such as Pipelines, Chains of Responsibility, and Dependency Graphs. It leverages dependency injection to automatically register and execute steps in a structured manner.

## Features

- **Pipelines**: Execute a series of steps in order, stopping on failure.
- **Chains of Responsibility**: Pass context through ordered handlers, optionally stopping the chain at any step.
- **Dependency Graphs**: Execute steps based on type dependencies.

## Prerequisites

- .NET 10.0 or later

## Installation

Install ContextFlow via NuGet:

```bash
dotnet add package ContextFlow
```

Or clone the repository and build locally:

```bash
git clone https://github.com/PedroAbreu1976/ContextFlow.git
cd ContextFlow
dotnet build
```

## Usage

### 1. Define a Context

Create a class that implements `IContext`:

```csharp
using ContextFlow;

public class MyContext : IContext
{
    public List<string> Data { get; set; } = [];
}
```

### 2. Register ContextFlow

Register flow services by scanning the assembly that contains your context and steps:

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ContextFlow;

var services = new ServiceCollection();
services.AddPipelines(Assembly.GetExecutingAssembly());
var provider = services.BuildServiceProvider();
```

### 3. Pipeline Flow

Use a pipeline when you need ordered processing and want to stop when a step returns `false`.

`Order` is critical in pipelines. Steps run from lowest to highest `Order`, so this is where you define your processing sequence (for example: validation -> enrichment -> persistence).

For Pipelines, implement `IPipelineStep<TContext>`:

```csharp
public class ValidatePipelineStep : IPipelineStep<MyContext>
{
    public int Order => 1;

    public Task<bool> ExecuteAsync(MyContext context, CancellationToken ct = default)
    {
        context.Data.Add("Validation completed.");
        return Task.FromResult(true);
    }
}

public class ProcessPipelineStep : IPipelineStep<MyContext>
{
    public int Order => 2;

    public Task<bool> ExecuteAsync(MyContext context, CancellationToken ct = default)
    {
        context.Data.Add("Processing completed.");
        return Task.FromResult(true); // return false here to stop following steps
    }
}
```

Execute the pipeline:

```csharp
var pipeline = provider.GetRequiredService<IPipeline<MyContext>>();
var context = new MyContext();
await pipeline.ExecuteAsync(context);
```

### 4. Chain of Responsibility Flow

Use a chain when each step decides if processing should continue by calling `next`.

`Order` is also important in chains. The handler with the lowest `Order` runs first and controls whether later handlers run.

Implement `IChainOfResponsibilityStep<TContext>`:

```csharp
public class LoggingChainStep : IChainOfResponsibilityStep<MyContext>
{
    public int Order => 1;

    public async Task ExecuteAsync(
        MyContext context,
        Func<MyContext, CancellationToken?, Task> next,
        CancellationToken? ct = default)
    {
        context.Data.Add("Request logged.");
        await next(context, ct);
    }
}

public class AuthorizationChainStep : IChainOfResponsibilityStep<MyContext>
{
    public int Order => 2;

    public async Task ExecuteAsync(
        MyContext context,
        Func<MyContext, CancellationToken?, Task> next,
        CancellationToken? ct = default)
    {
        var isAuthorized = true;
        context.Data.Add("Authorization checked.");

        if (isAuthorized)
        {
            await next(context, ct);
        }
        // If not authorized, do not call next and the chain stops here.
    }
}
```

Execute the chain:

```csharp
var chain = provider.GetRequiredService<IChainOfResponsibility<MyContext>>();
var context = new MyContext();
await chain.ExecuteAsync(context);
```

### 5. Dependency Graph Flow

Use a dependency graph when steps must run only after specific previous step types complete.

Implement `IDependencyGraphStep<TContext>`:

```csharp
public class FirstGraphStep : IDependencyGraphStep<MyContext>
{
    public Type[] PreviousTypes => [];

    public Task<bool> ExecuteAsync(MyContext context, CancellationToken? ct = default)
    {
        context.Data.Add("First graph step.");
        return Task.FromResult(true);
    }
}

public class SecondGraphStep : IDependencyGraphStep<MyContext>
{
    public Type[] PreviousTypes => [typeof(FirstGraphStep)];

    public Task<bool> ExecuteAsync(MyContext context, CancellationToken? ct = default)
    {
        context.Data.Add("Second graph step.");
        return Task.FromResult(true);
    }
}
```

Execute the graph:

```csharp
var graph = provider.GetRequiredService<IDependencyGraph<MyContext>>();
var context = new MyContext();
await graph.ExecuteAsync(context);
```

## Examples

See the `ContextFlow.Demo` project for complete examples of Pipelines, Chains, and Graphs.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.
