namespace SMTPterodactyl.Infrastructure.Database
{
    using Microsoft.EntityFrameworkCore;
    using SMTPterodactyl.Core.Entities;
    using SMTPterodactyl.Core.Entities.Channels;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<ConsoleChannel> ConsoleChannels { get; set; }

        public DbSet<FileChannel> FileChannels { get; set; }

        public DbSet<TelegramChannel> TelegramChannels { get; set; }

        public DbSet<Flow> Flows { get; set; }
    }
}
