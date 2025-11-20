namespace SMTPterodactyl.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using SMTPterodactyl.Application.Abstractions;
using SMTPterodactyl.Core.Channels;
using SMTPterodactyl.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ChannelRepository : IChannelRepository
{
    private readonly ApplicationDbContext db;

    public ChannelRepository(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<Channel?> GetByIdAsync(Guid channelId, CancellationToken ct)
    {
        return await this.db.Channels.FirstOrDefaultAsync(e => e.Id == channelId, ct);
    }

    public async Task<IReadOnlyDictionary<Guid, Channel>> GetByIdsAsync(IEnumerable<Guid> channelIds, CancellationToken ct)
    {
        var ids = channelIds.ToList();
        var list = await this.db.Channels.Where(e => ids.Contains(e.Id)).ToListAsync(ct);
        return list.ToDictionary(e => e.Id, e => e);
    }
}
