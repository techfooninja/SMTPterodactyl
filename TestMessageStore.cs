namespace SMTPterodactyl
{
    using MimeKit;
    using SmtpServer;
    using SmtpServer.Protocol;
    using SmtpServer.Storage;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;

    internal class TestMessageStore : IMessageStore
    {
        private readonly string token;
        private readonly long chatId;
        private ITelegramBotClient client;

        public TestMessageStore(string telegramToken, long telegramChatId)
        {
            this.token = telegramToken;
            this.chatId = telegramChatId;
            this.client = new TelegramBotClient(this.token);

            // TODO: Add logic to reply with the current chatId
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

            await this.client.SendMessage(new Telegram.Bot.Types.ChatId(this.chatId), $"{message.Subject}\r\n\r\n{message.TextBody}");

            return SmtpResponse.Ok;
        }
    }
}
