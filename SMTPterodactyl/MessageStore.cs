namespace SMTPterodactyl
{
    using MimeKit;
    using SmtpServer;
    using SmtpServer.Protocol;
    using SmtpServer.Storage;
    using SMTPterodactyl.Core.Interfaces.Repositories;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    internal class MessageStore : IMessageStore
    {
        private readonly IFlowRepository flowRepository;

        public MessageStore(IFlowRepository flowRepository)
        {
            this.flowRepository = flowRepository;
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

            var flows = await this.flowRepository.GetAllAsync();

            foreach (var flow in flows)
            {
                await flow.HandleMessageAsync(message);
            }

            return SmtpResponse.Ok;
        }
    }
}
