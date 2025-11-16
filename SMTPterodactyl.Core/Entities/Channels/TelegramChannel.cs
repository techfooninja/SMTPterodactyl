namespace SMTPterodactyl.Core.Entities.Channels
{
    using MimeKit;
    using System;
    using System.Threading.Tasks;

    public class TelegramChannel : Channel
    {
        public TelegramChannel(
            Guid id,
            string name,
            string botToken,
            long chatId) : base(id, name)
        {
            BotToken = botToken;
            ChatId = chatId;
        }

        public string BotToken { get; private set; }

        public long ChatId { get; private set; }

        public event EventHandler<MimeMessage>? OnHandleMessage;

        public async override Task HandleMessageAsync(MimeMessage message)
        {
            if (OnHandleMessage != null)
            {
                OnHandleMessage.Invoke(this, message);
            }

            await Task.CompletedTask;
        }
    }
}
