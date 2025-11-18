namespace SMTPterodactyl
{
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    using SmtpServer;
    using SmtpServer.Protocol;
    using SmtpServer.Storage;
    using SMTPterodactyl.Infrastructure.Database;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    internal class MessageStore : IMessageStore
    {
        private readonly ApplicationDbContext db;

        public MessageStore(ApplicationDbContext db)
        {
            this.db = db;
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

            var flows = await this.db.Flows.Include(f => f.Channels).ToListAsync();

            foreach (var flow in flows)
            {
                await flow.HandleMessageAsync(message);
            }

            return SmtpResponse.Ok;
        }
    }
}
