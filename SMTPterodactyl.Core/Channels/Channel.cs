namespace SMTPterodactyl.Core.Channels;

using System;

public abstract class Channel
{
    public Channel(Guid id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public Guid Id { get; private set; }

    public bool IsActive { get; set; }

    public string Name { get; set; }
}
