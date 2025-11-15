namespace SMTPterodactyl.Core.Channels
{
    using MimeKit;
    using System;
    using System.Threading.Tasks;

    public class TelegramChannel : IChannel
    {
        public string? Name { get; set; }

        public string? BotToken { get; set; }

        public long ChatId { get; set; }

        public event EventHandler<MimeMessage>? OnHandleMessage;

        public async Task HandleMessage(MimeMessage message)
        {
            if (this.OnHandleMessage != null)
            {
                this.OnHandleMessage.Invoke(this, message);
            }

            await Task.CompletedTask;
        }
    }
}
