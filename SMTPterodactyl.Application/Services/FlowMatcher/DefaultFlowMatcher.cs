namespace SMTPterodactyl.Application.Services.FlowMatcher;

using SMTPterodactyl.Core.Entities;
using SMTPterodactyl.Core.Messages;
using System.Collections.Generic;
using System.Linq;

public class DefaultFlowMatcher : IFlowMatcher
{
    public IReadOnlyList<Flow> Match(MessageEnvelope envelope, IReadOnlyList<Flow> flows)
    {
        return flows
            .Where(f => f.IsEnabled)
            .OrderBy(f => f.Order)
            .ToList();
    }
}
