using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.Storage;
using SMTPterodactyl;
using SMTPterodactyl.Core.Channels;
using SMTPterodactyl.Core.Flows;
using SMTPterodactyl.Persistence;
using SMTPterodactyl.Persistence.Json;
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
builder.Services.AddSingleton<IDataStore<IChannel>>(new JsonDataStore<IChannel>(Path.Combine(Environment.CurrentDirectory, "channels.json")));
builder.Services.AddSingleton<IDataStore<IFlow>>(new JsonDataStore<IFlow>(Path.Combine(Environment.CurrentDirectory, "flows.json"), new string[] { nameof(IFlow.Channels) }));
builder.Services.AddSingleton<IDictionary<string, ITelegramBotClient>>(new Dictionary<string, ITelegramBotClient>());
builder.Services.AddHostedService<SmtpHostedService>();

var host = builder.Build();
await InitializeTelegram();
await InitializeFlows();
await host.RunAsync();

async Task InitializeTelegram()
{
    // Hook up Telegram channels with bots
    var channelStore = host.Services.GetService<IDataStore<IChannel>>();

    if (channelStore == null)
    {
        throw new KeyNotFoundException($"No instance of {typeof(IDataStore<IChannel>).Name} was found. Did you forget to register it?");
    }

    var telegramBots = host.Services.GetService<IDictionary<string, ITelegramBotClient>>();

    if (telegramBots == null)
    {
        throw new KeyNotFoundException($"No instance of {typeof(IDictionary<string, ITelegramBotClient>).Name} was found. Did you forget to register it?");
    }

    var channels = await channelStore.GetAsync();
    foreach (var channel in channels)
    {
        if (channel is TelegramChannel telegramChannel)
        {
            if (string.IsNullOrWhiteSpace(telegramChannel.BotToken))
            {
                continue;
            }

            if (!telegramBots.ContainsKey(telegramChannel.BotToken))
            {
                var bot = new TelegramBotClient(telegramChannel.BotToken);
                bot.OnMessage += async (msg, type) =>
                {
                    if (string.Equals(msg.Text, "/start", StringComparison.OrdinalIgnoreCase))
                    {
                        await bot.SendMessage(msg.Chat, $"Your ID is {msg.Chat.Id}");
                    }
                };

                telegramBots[telegramChannel.BotToken] = bot;
            }

            telegramChannel.OnHandleMessage += async (s, msg) =>
            {
                var bot = telegramBots[telegramChannel.BotToken];
                await bot.SendMessage(new Telegram.Bot.Types.ChatId(telegramChannel.ChatId), $"{msg.Subject}\r\n\r\n{msg.TextBody}");
            };
        }
    }
}

async Task InitializeFlows()
{
    var channelStore = host.Services.GetService<IDataStore<IChannel>>();

    if (channelStore == null)
    {
        throw new KeyNotFoundException($"No instance of {typeof(IDataStore<IChannel>).Name} was found. Did you forget to register it?");
    }

    var flowStore = host.Services.GetService<IDataStore<IFlow>>();

    if (flowStore == null)
    {
        throw new KeyNotFoundException($"No instance of {typeof(IDataStore<IFlow>).Name} was found. Did you forget to register it?");
    }

    var channels = await channelStore.GetAsync();
    var flows = await flowStore.GetAsync();

    foreach (var iflow in flows)
    {
        if (iflow is Flow flow)
        {
            foreach (var channelName in flow.ChannelNames)
            {
                var channel = channels.FirstOrDefault(c => string.Equals(c.Name, channelName, StringComparison.Ordinal));

                if (channel == null)
                {
                    throw new KeyNotFoundException($"No channel found with name '{channelName}'");
                }

                flow.Channels.Add(channel);
            }
        }
    }
}