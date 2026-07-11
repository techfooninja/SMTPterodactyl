namespace SMTPterodactyl;

using MimeKit;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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
            var chat = new ChatId(this.chatId);
            using var memoryStream = new MemoryStream();
            await message.WriteToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            await this.bot.SendDocument(chat, InputFile.FromStream(memoryStream, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.eml"), $"To: {message.To}\r\nFrom: {message.From}\r\nSubject: {message.Subject}\r\n\r\n{message.TextBody}");
        }
    }
}
