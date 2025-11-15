namespace SMTPterodactyl.Core.Flows
{
    using MimeKit;
    using SMTPterodactyl.Core.Channels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFlow
    {
        public IList<IChannel> Channels { get; set; }

        public Task HandleMessageAsync(MimeMessage message);
    }
}
