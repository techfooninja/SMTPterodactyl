namespace SMTPterodactyl;

internal class MessageHandlingSettings
{
    public bool OutputToConsole { get; set; }

    public string? SaveDirectory { get; set; }

    public string? TelegramBotToken { get; set; }

    public long? TelegramChatId { get; set; }
}
