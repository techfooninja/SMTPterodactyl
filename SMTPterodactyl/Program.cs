using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.Storage;
using SMTPterodactyl;
using SMTPterodactyl.Core.Entities.Channels;
using SMTPterodactyl.Infrastructure.Database;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(
    new SmtpServerOptionsBuilder()
        .ServerName("localhost")
        .Port(25, 587)
        .Build());

Console.WriteLine($"[DB] Connection string resolved to {builder.Configuration.GetConnectionString("ApplicationDatabase")}");
Console.WriteLine($"[DB] CurrentDirectory: {Environment.CurrentDirectory}");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("ApplicationDatabase"), b => b.MigrationsAssembly(nameof(SMTPterodactyl))));
builder.Services.AddSingleton(UserAuthenticator.Default);
builder.Services.AddSingleton(MailboxFilter.Default);
builder.Services.AddScoped<IMessageStore, SMTPterodactyl.MessageStore>();
builder.Services.AddSingleton<SmtpServer.SmtpServer>();
builder.Services.AddSingleton<IDictionary<string, ITelegramBotClient>>(new Dictionary<string, ITelegramBotClient>());
builder.Services.AddHostedService<SmtpHostedService>();

var host = builder.Build();
await InitializeDatabase();
await InitializeTelegram();
await host.RunAsync();

async Task InitializeDatabase()
{
    var dbContext = host.Services.GetService<ApplicationDbContext>();

    if (dbContext == null)
    {
        throw new KeyNotFoundException($"No instance of {nameof(ApplicationDbContext)} was found. Did you forget to register it?");
    }

    await dbContext.Database.MigrateAsync();
}

async Task InitializeTelegram()
{
    // Hook up Telegram channels with bots
    var db = host.Services.GetService<ApplicationDbContext>();

    if (db == null)
    {
        throw new KeyNotFoundException($"No instance of {nameof(ApplicationDbContext)} was found. Did you forget to register it?");
    }

    var telegramBots = host.Services.GetService<IDictionary<string, ITelegramBotClient>>();

    if (telegramBots == null)
    {
        throw new KeyNotFoundException($"No instance of {typeof(IDictionary<string, ITelegramBotClient>).Name} was found. Did you forget to register it?");
    }

    var channels = await db.Channels.ToListAsync();
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