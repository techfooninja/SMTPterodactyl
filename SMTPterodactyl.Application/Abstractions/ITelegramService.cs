namespace SMTPterodactyl.Application.Abstractions;

using SMTPterodactyl.Core.Messages;
using System.Threading.Tasks;

public interface ITelegramService
{
    public Task SendAsync(string botToken, long chatId, MessageEnvelope message);
}
