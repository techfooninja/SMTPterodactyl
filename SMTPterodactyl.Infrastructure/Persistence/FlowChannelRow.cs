namespace SMTPterodactyl.Infrastructure.Persistence;

using System;

public sealed class FlowChannelRow
{
    public Guid FlowId { get; set; }

    public Guid ChannelId { get; set; }
}
