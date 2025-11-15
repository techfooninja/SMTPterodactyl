namespace SMTPterodactyl.Core.Channels
{
    using MimeKit;
    using System.Threading.Tasks;

    public interface IChannel
    {
        public string? Name { get; set; }

        public Task HandleMessageAsync(MimeMessage message);
    }
}
