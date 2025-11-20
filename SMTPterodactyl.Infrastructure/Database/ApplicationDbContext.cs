namespace SMTPterodactyl.Infrastructure.Database
{
    using Microsoft.EntityFrameworkCore;
    using SMTPterodactyl.Core.Channels;
    using SMTPterodactyl.Infrastructure.Persistence;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=core.db");
            base.OnConfiguring(optionsBuilder);;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FlowRow>(b =>
            {
                b.HasKey(f => f.Id);
                b.Property(f => f.Name).HasMaxLength(200).IsRequired();
                b.Property(f => f.Order).IsRequired();
                b.Property(f => f.IsEnabled).IsRequired();
            });

            modelBuilder.Entity<FlowChannelRow>(b =>
            {
                b.HasKey(fc => new { fc.FlowId, fc.ChannelId });

                b.HasOne<FlowRow>()
                    .WithMany()
                    .HasForeignKey(fc => fc.FlowId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<Channel>()
                    .WithMany()
                    .HasForeignKey(fc => fc.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<Channel> Channels => this.Set<Channel>();

        public DbSet<ConsoleChannel> ConsoleChannels => this.Set<ConsoleChannel>();

        public DbSet<FileChannel> FileChannels => this.Set<FileChannel>();

        public DbSet<TelegramChannel> TelegramChannels => this.Set<TelegramChannel>();

        public DbSet<TelegramBotConfiguration> TelegramBotConfigurations => this.Set<TelegramBotConfiguration>();

        public DbSet<FlowRow> Flows => this.Set<FlowRow>();

        public DbSet<FlowChannelRow> FlowChannels => this.Set<FlowChannelRow>();
    }
}
