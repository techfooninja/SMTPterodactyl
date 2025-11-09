using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.Storage;
using SMTPterodactyl;
using SMTPterodactyl.Core.Channels;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(
    new SmtpServerOptionsBuilder()
        .ServerName("localhost")
        .Port(25, 587)
        .Build());
builder.Services.AddSingleton(UserAuthenticator.Default);
builder.Services.AddSingleton(MailboxFilter.Default);
builder.Services.AddSingleton<IMessageStore, SMTPterodactyl.MessageStore>();
builder.Services.AddSingleton<SmtpServer.SmtpServer>();

// TODO: Add logic to have Telegram bots reply with the current chatId when receiving a message
builder.Services.AddKeyedSingleton<ITelegramBotClient>(args[0], new TelegramBotClient(args[0]));

// TODO: We need to be able to support multiple Telegram channels eventually
builder.Services.AddTransient<IChannel>(services => new TelegramChannel(args[0], long.Parse(args[1]), services));

builder.Services.AddHostedService<SmtpHostedService>();

var host = builder.Build();
await host.RunAsync();