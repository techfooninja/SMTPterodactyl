namespace SMTPterodactyl.Core.Channels
{
    using Microsoft.Extensions.DependencyInjection;
    using MimeKit;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Telegram.Bot;

    public class TelegramChannel : IChannel
    {
        private ITelegramBotClient botClient;
        private readonly long chatId;

        public TelegramChannel(string botToken, long chatId, IServiceProvider services)
        {
            this.chatId = chatId;
            var bot = services.GetKeyedService<ITelegramBotClient>(botToken);

            if (bot == null)
            {
                throw new KeyNotFoundException($"Could not find an instance of {nameof(ITelegramBotClient)} with a key of {botToken}");
            }

            this.botClient = bot;
        }

        public async Task HandleMessage(MimeMessage message)
        {
            await this.botClient.SendMessage(new Telegram.Bot.Types.ChatId(this.chatId), $"{message.Subject}\r\n\r\n{message.TextBody}");
        }
    }
}
