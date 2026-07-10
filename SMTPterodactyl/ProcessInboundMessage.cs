namespace SMTPterodactyl;

using MimeKit;
using System.Threading.Tasks;

internal class ProcessInboundMessageHandler(
    MessageHandlingSettings messageHandlingSettings,
    TelegramService telegramService)
{
    public async Task HandleAsync(MimeMessage message, CancellationToken ct = default)
    {
        if (messageHandlingSettings.OutputToConsole)
        {
            Console.WriteLine($"To: {message.To}\r\nFrom: {message.From}\r\nSubject: {message.Subject}\r\nBody: {message.TextBody}\r\n");
        }

        var directory = messageHandlingSettings.SaveDirectory;
        if (!string.IsNullOrWhiteSpace(directory))
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await message.WriteToAsync(Path.Combine(directory, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Guid.NewGuid()}.msg"));
        }

        await telegramService.SendAsync(message);
    }
}