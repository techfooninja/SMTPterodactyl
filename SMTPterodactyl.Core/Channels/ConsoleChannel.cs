namespace SMTPterodactyl.Core.Channels
{
    using MimeKit;
    using System;
    using System.Threading.Tasks;

    public class ConsoleChannel : IChannel
    {
        public string? Name { get; set; }

        public Task HandleMessageAsync(MimeMessage message)
        {
            Console.WriteLine($"To: {message.To}\r\nFrom: {message.From}\r\nSubject: {message.Subject}\r\nBody: {message.TextBody}\r\n");
            return Task.CompletedTask;
        }
    }
}
