using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMTPterodactyl.Infrastructure;
using SMTPterodactyl.Infrastructure.Database;

var builder = Host.CreateApplicationBuilder(args);
Console.WriteLine($"[DB] Connection string resolved to {builder.Configuration.GetConnectionString("ApplicationDatabase")}");
Console.WriteLine($"[DB] CurrentDirectory: {Environment.CurrentDirectory}");

builder.Services.AddInfrastructure(builder.Configuration, true);

var host = builder.Build();
await InitializeDatabase();
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