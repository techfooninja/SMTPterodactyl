namespace SMTPterodactyl.Core.Entities.Channels
{
    using MimeKit;
    using System;
    using System.Threading.Tasks;

    public class ConsoleChannel : Channel
    {
        public ConsoleChannel(Guid id, string name) : base(id, name)
        {
        }

        public override Task HandleMessageAsync(MimeMessage message)
        {
            Console.WriteLine($"To: {message.To}\r\nFrom: {message.From}\r\nSubject: {message.Subject}\r\nBody: {message.TextBody}\r\n");
            return Task.CompletedTask;
        }
    }
}
