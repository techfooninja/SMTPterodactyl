namespace SMTPterodactyl.Core.Channels;

using System;

public class TelegramBotConfiguration
{
    public TelegramBotConfiguration(
        Guid id,
        string botToken,
        string name)
    {
        this.Id = id;
        this.BotToken = botToken;
        this.Name = name;
    }

    public string BotToken { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }
}
