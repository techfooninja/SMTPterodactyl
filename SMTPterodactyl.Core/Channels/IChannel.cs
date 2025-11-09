namespace SMTPterodactyl.Core.Channels
{
    using MimeKit;
    using System.Threading.Tasks;

    public interface IChannel
    {
        public Task HandleMessage(MimeMessage message);
    }
}
