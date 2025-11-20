namespace SMTPterodactyl.Application.Services.FlowMatcher;

using SMTPterodactyl.Core.Entities;
using SMTPterodactyl.Core.Messages;
using System.Collections.Generic;

public interface IFlowMatcher
{
    IReadOnlyList<Flow> Match(MessageEnvelope envelope, IReadOnlyList<Flow> flows);
}
