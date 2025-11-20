namespace SMTPterodactyl.Core.Entities;

using SMTPterodactyl.Core.Flows;
using System;
using System.Collections.Generic;

public class Flow
{
    public Flow(Guid id, bool isEnabled, string name, int order)
    {
        this.Id = id;
        this.IsEnabled = isEnabled;
        this.Name = name;
        this.Order = order;
    }

    public Guid Id { get; private set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; }

    public int Order { get; set; }

    public virtual ICollection<FlowChannelLink> Channels { get; set; } = new HashSet<FlowChannelLink>();
}
