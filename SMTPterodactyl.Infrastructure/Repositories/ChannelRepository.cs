namespace SMTPterodactyl.Infrastructure.Repositories
{
    using SMTPterodactyl.Core.Entities.Channels;
    using SMTPterodactyl.Core.Interfaces.Repositories;
    using SMTPterodactyl.Infrastructure.Database;

    public class ChannelRepository : BaseRepository<Channel>, IChannelRepository
    {
        public ChannelRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
