// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using ContextFlow;
using System.Reflection;
using ContextFlow.Demo.ChainDemo;
using ContextFlow.Demo.PipelineDemo;
using ContextFlow.Demo.GraphDemo;
using System.Net;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");
var provider = new ServiceCollection()
    .AddLogging(b=>b.AddConsole())
    .AddPipelines([Assembly.GetExecutingAssembly()], opt=> { opt.LogExecution = true; })
    .BuildServiceProvider();

var pipeline = provider.GetRequiredService<IPipeline<LoanPreApprovalPipelineContext>>();
var pipelineContext = new LoanPreApprovalPipelineContext();
await pipeline.ExecuteAsync(pipelineContext);
Console.WriteLine(pipelineContext.ToString());

var chain = provider.GetRequiredService<IChainOfResponsibility<LoanPreApprovalChainContext>>();
var chainContext = new LoanPreApprovalChainContext();
await chain.ExecuteAsync(chainContext);
Console.WriteLine(chainContext.ToString());

var graph = provider.GetRequiredService<IDependencyGraph<LoanPreApprovalGraphContext>>();
var graphContext = new LoanPreApprovalGraphContext();
await graph.ExecuteAsync(graphContext);
Console.WriteLine(graphContext.ToString());