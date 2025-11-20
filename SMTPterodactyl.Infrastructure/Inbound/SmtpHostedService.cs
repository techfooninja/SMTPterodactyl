namespace SMTPterodactyl.Infrastructure.Inbound
{
    using Microsoft.Extensions.Hosting;
    using SmtpServer;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SmtpHostedService : IHostedService
    {
        private readonly SmtpServer smtpServer;

        public SmtpHostedService(SmtpServer smtpServer)
        {
            this.smtpServer = smtpServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting SMTP Server...\r\n");
            return this.smtpServer.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.smtpServer != null)
            {
                this.smtpServer.Shutdown();
                await this.smtpServer.ShutdownTask;
            }
        }
    }
}
