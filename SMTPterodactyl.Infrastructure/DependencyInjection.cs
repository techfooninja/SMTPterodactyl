namespace SMTPterodactyl.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.Storage;
using SMTPterodactyl.Application.Abstractions;
using SMTPterodactyl.Application.Services.FlowMatcher;
using SMTPterodactyl.Application.UseCases;
using SMTPterodactyl.Infrastructure.Database;
using SMTPterodactyl.Infrastructure.Inbound;
using SMTPterodactyl.Infrastructure.Repositories;
using SMTPterodactyl.Infrastructure.Services;
using System.Collections.Generic;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool addHostedServices = true)
    {
        // Database
        var conn = configuration.GetConnectionString("ApplicationDatabase") ?? "Data Source=core.db";
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(conn));

        // Repositories
        services.AddScoped<IFlowRepository, FlowRepository>();
        services.AddScoped<IChannelRepository, ChannelRepository>();

        // Services
        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        services.AddSingleton<IFlowMatcher, DefaultFlowMatcher>();
        services.AddSingleton<ITelegramService, TelegramService>();
        services.AddSingleton(UserAuthenticator.Default);
        services.AddSingleton(MailboxFilter.Default);
        services.AddScoped<IMessageStore, SmtpMessageStore>();
        services.AddScoped<SmtpServer>();
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

        // Use cases
        services.AddScoped<ProcessInboundMessageHandler>();        

        // Hosted services
        if (addHostedServices)
        {
            services.AddHostedService<SmtpHostedService>();
        }

        return services;
    }
}
