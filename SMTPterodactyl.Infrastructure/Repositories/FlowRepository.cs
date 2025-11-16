namespace SMTPterodactyl.Infrastructure.Repositories
{
    using SMTPterodactyl.Core.Entities;
    using SMTPterodactyl.Core.Interfaces.Repositories;
    using SMTPterodactyl.Infrastructure.Database;

    public class FlowRepository : BaseRepository<Flow>, IFlowRepository
    {
        public FlowRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
