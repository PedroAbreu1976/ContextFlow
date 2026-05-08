using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ContextFlow.Options;

public class ContextFlowConfigurationSetup(IServiceProvider serviceProvider) : IConfigureOptions<ContextFlowConfiguration>
{
    private const string SectionName = "ConfigFlow";

    public void Configure(ContextFlowConfiguration options)
    {
        serviceProvider.GetService<IConfiguration>()?
            .GetSection(SectionName)
            .Bind(options);
    }
}
