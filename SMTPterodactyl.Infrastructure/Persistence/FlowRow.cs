namespace SMTPterodactyl.Infrastructure.Persistence;

using System;

public sealed class FlowRow
{
    public Guid Id { get; set; }

    public bool IsEnabled { get; set; }

    public string Name { get; set; } = default!;

    public int Order { get; set; }
}
