namespace SMTPterodactyl.Infrastructure.Inbound
{
    using MimeKit;
    using SmtpServer;
    using SmtpServer.Protocol;
    using SmtpServer.Storage;
    using SMTPterodactyl.Application.UseCases;
    using SMTPterodactyl.Core.Messages;
    using SMTPterodactyl.Infrastructure.Database;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SmtpMessageStore : IMessageStore
    {
        private readonly ApplicationDbContext db;
        private readonly ProcessInboundMessageHandler handler;

        public SmtpMessageStore(
            ApplicationDbContext db,
            ProcessInboundMessageHandler handler)
        {
            this.db = db;
            this.handler = handler;
        }

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
            var envelope = ConvertToEnvelope(message);
            await this.handler.HandleAsync(new ProcessInboundMessageCommand(envelope), cancellationToken);
            return SmtpResponse.Ok;
        }

        private static MessageEnvelope ConvertToEnvelope(MimeMessage msg)
        {
            var messageId = string.IsNullOrWhiteSpace(msg.MessageId) ? Guid.NewGuid().ToString("N") : msg.MessageId;
            var from = msg.From.Mailboxes.FirstOrDefault()?.Address ?? "unknown@unknown";
            var to = msg.To.Mailboxes.Select(m => m.Address).ToList();
            var subject = msg.Subject;
            var text = msg.TextBody;
            var html = msg.HtmlBody;
            var receivedAt = DateTimeOffset.UtcNow;

            var media = new List<MessageMedia>();
            foreach (var part in msg.BodyParts)
            {
                if (part is MimePart mp && mp.IsAttachment)
                {
                    using var ms = new MemoryStream();
                    mp.Content?.DecodeTo(ms);
                    media.Add(new MessageMedia(mp.FileName ?? "attachment", mp.ContentType.MimeType, ms.ToArray()));
                }
            }

            var headers = msg.Headers.ToDictionary(h => h.Field, h => h.Value);
            return new MessageEnvelope(messageId, from, to, subject, text, html, receivedAt, media, headers);
        }
    }
}
