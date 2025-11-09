using SmtpServer;
using SmtpServer.ComponentModel;
using SMTPterodactyl;

Console.WriteLine($"Args: {string.Join(", ", args)}");

var options = new SmtpServerOptionsBuilder()
    .ServerName("localhost")
    .Port(25, 587)
    .Build();

var serviceProvider = new ServiceProvider();
serviceProvider.Add(new TestMessageStore(args[0], long.Parse(args[1])));
var smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);
Console.WriteLine("Starting SMTP Server...\r\n");
await smtpServer.StartAsync(CancellationToken.None);
Console.WriteLine("Ending SMTP Server");
Console.ReadLine();