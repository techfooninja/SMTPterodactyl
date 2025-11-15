namespace SMTPterodactyl.Core.Flows
{
    using MimeKit;
    using SMTPterodactyl.Core.Channels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Flow : IFlow
    {
        public Flow()
        {
            this.ChannelNames = new List<string>();
            this.Channels = new List<IChannel>();
        }

        public IList<string> ChannelNames { get; set; }

        public IList<IChannel> Channels { get; set; }

        public async Task HandleMessageAsync(MimeMessage message)
        {
            foreach (var channel in this.Channels)
            {
                await channel.HandleMessageAsync(message);
            }
        }
    }
}
