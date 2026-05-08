using Microsoft.Extensions.Options;

namespace ContextFlow.Options;

public class ContextFlowConfigurationOptions(ContextFlowConfiguration value) : IOptions<ContextFlowConfiguration>
{
    public ContextFlowConfiguration Value => value;
}
