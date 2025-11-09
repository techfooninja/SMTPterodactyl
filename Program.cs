using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.Storage;
using SMTPterodactyl;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(
    new SmtpServerOptionsBuilder()
        .ServerName("localhost")
        .Port(25, 587)
        .Build());
builder.Services.AddSingleton(UserAuthenticator.Default);
builder.Services.AddSingleton(MailboxFilter.Default);
builder.Services.AddSingleton<IMessageStore>(new TestMessageStore(args[0], long.Parse(args[1])));
builder.Services.AddSingleton<SmtpServer.SmtpServer>();
builder.Services.AddHostedService<SmtpHostedService>();

var host = builder.Build();
await host.RunAsync();