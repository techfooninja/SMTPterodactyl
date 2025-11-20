namespace SMTPterodactyl.Core.Flows;

using System;

public class FlowChannelLink
{
    public FlowChannelLink(Guid flowId, Guid endpointId)
    {
        this.FlowId = flowId;
        this.EndpointId = endpointId;
    }

    public Guid FlowId { get; }

    public Guid EndpointId { get; }
}
