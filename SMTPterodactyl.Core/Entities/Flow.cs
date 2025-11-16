namespace SMTPterodactyl.Core.Entities
{
    using MimeKit;
    using SMTPterodactyl.Core.Entities.Channels;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Flow : Entity
    {
        public Flow(Guid id) : base(id)
        {
        }

        public virtual ICollection<Channel> Channels { get; set; } = new HashSet<Channel>();

        public async Task HandleMessageAsync(MimeMessage message)
        {
            foreach (var channel in Channels)
            {
                await channel.HandleMessageAsync(message);
            }
        }
    }
}
