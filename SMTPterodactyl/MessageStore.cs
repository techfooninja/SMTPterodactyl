namespace SMTPterodactyl
{
    using MimeKit;
    using SmtpServer;
    using SmtpServer.Protocol;
    using SmtpServer.Storage;
    using SMTPterodactyl.Core.Flows;
    using SMTPterodactyl.Persistence;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    internal class MessageStore : IMessageStore
    {
        private readonly IDataStore<IFlow> flowStore;

        public MessageStore(IDataStore<IFlow> flowStore)
        {
            this.flowStore = flowStore;
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
            Console.WriteLine($"To: {message.To}\r\nFrom: {message.From}\r\nSubject: {message.Subject}\r\nBody: {message.TextBody}\r\n");

            await message.WriteToAsync(Path.Combine(@"C:\temp", $"{Guid.NewGuid()}.msg"));

            var flows = await this.flowStore.GetAsync();

            foreach (var flow in flows)
            {
                await flow.HandleMessageAsync(message);
            }

            return SmtpResponse.Ok;
        }
    }
}
