namespace SMTPterodactyl.Core.Channels;

using System;

public class TelegramChannel : Channel
{
    public TelegramChannel(
        Guid id,
        string name,
        Guid botConfigurationId,
        long chatId) : base(id, name)
    {
        BotConfigurationId = botConfigurationId;
        ChatId = chatId;
    }

    public long ChatId { get; private set; }

    public Guid BotConfigurationId { get; set; }

    public virtual TelegramBotConfiguration? BotConfiguration { get; set; }
}
