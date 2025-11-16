namespace SMTPterodactyl.Core.Entities.Channels
{
    using MimeKit;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class Channel : Entity
    {
        public Channel(Guid id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; set; }

        public virtual ICollection<Flow> Flows { get; private set; } = new HashSet<Flow>();

        public abstract Task HandleMessageAsync(MimeMessage message);
    }
}
