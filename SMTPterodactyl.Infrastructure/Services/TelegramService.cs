namespace SMTPterodactyl.Infrastructure.Services;

using SMTPterodactyl.Application.Abstractions;
using SMTPterodactyl.Core.Messages;
using SMTPterodactyl.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

public class TelegramService : ITelegramService
{
    private readonly Dictionary<string, ITelegramBotClient> bots;

    public TelegramService(ApplicationDbContext db)
    {
        this.bots = new Dictionary<string, ITelegramBotClient>();

        foreach (var botConfig in db.TelegramBotConfigurations.ToList())
        {
            if (!this.bots.ContainsKey(botConfig.BotToken))
            {
                var bot = new TelegramBotClient(botConfig.BotToken);
                bot.OnMessage += async (msg, type) =>
                {
                    if (string.Equals(msg.Text, "/start", StringComparison.OrdinalIgnoreCase))
                    {
                        await bot.SendMessage(msg.Chat, $"Your ID is {msg.Chat.Id}");
                    }
                };

                this.bots[botConfig.BotToken] = bot;
            }
        }
    }

    public async Task SendAsync(string botToken, long chatId, MessageEnvelope message)
    {
        var bot = this.bots[botToken];
        await bot.SendMessage(new Telegram.Bot.Types.ChatId(chatId), $"{message.Subject}\r\n\r\n{message.PlainTextBody}");
    }
}
