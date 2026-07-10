using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.Storage;
using SMTPterodactyl;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
services.Configure<MessageHandlingSettings>(configuration.GetSection("MessageHandlingSettings"));

services.AddSingleton<TelegramService>();
services.AddSingleton(UserAuthenticator.Default);
services.AddSingleton(MailboxFilter.Default);
services.AddScoped<IMessageStore, SmtpMessageStore>();

services.AddScoped<SmtpServer.SmtpServer>();
services.AddSingleton(services =>
{
    var options = services.GetService<IOptions<SmtpOptions>>();

    if (options == null)
    {
        throw new KeyNotFoundException($"No instance of {nameof(SmtpOptions)} was found. Did you forget to register it?");
    }

    return new SmtpServerOptionsBuilder()
        .ServerName(options.Value.ServerName)
        .Port(options.Value.Ports)
        .Build();
});

services.AddScoped<ProcessInboundMessageHandler>();
services.AddHostedService<SmtpHostedService>();

var host = builder.Build();
await host.RunAsync();
