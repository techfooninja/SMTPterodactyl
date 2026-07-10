namespace SMTPterodactyl;

using MimeKit;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal class SmtpMessageStore(ProcessInboundMessageHandler handler) : IMessageStore
{
    public async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();

        var position = buffer.GetPosition(0);
        while (buffer.TryGet(ref position, out var memory))
        {
            await stream.WriteAsync(memory, cancellationToken);
        }

        stream.Position = 0;
        var message = await MimeMessage.LoadAsync(stream, cancellationToken);
        await handler.HandleAsync(message, cancellationToken);
        return SmtpResponse.Ok;
    }
}