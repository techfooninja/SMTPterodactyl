namespace SMTPterodactyl;

using MimeKit;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

internal class TelegramService
{
    private TelegramBotClient? bot;
    private long chatId;

    public TelegramService(MessageHandlingSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.TelegramBotToken))
        {
            this.bot = new TelegramBotClient(settings.TelegramBotToken);
            this.bot.OnMessage += async (msg, type) =>
            {
                if (string.Equals(msg.Text, "/start", StringComparison.OrdinalIgnoreCase))
                {
                    await bot.SendMessage(msg.Chat, $"Your ID is {msg.Chat.Id}");
                }
            };

            this.chatId = settings.TelegramChatId ?? 0;
        }
    }

    public async Task SendAsync(MimeMessage message)
    {
        if (this.bot != null)
        {
            await this.bot.SendMessage(new Telegram.Bot.Types.ChatId(this.chatId), $"{message.Subject}\r\n\r\n{message.TextBody}");
        }
    }
}
