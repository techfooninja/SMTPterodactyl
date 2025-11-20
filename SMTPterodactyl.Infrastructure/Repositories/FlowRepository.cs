namespace SMTPterodactyl.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using SMTPterodactyl.Application.Abstractions;
using SMTPterodactyl.Core.Entities;
using SMTPterodactyl.Core.Flows;
using SMTPterodactyl.Infrastructure.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class FlowRepository : IFlowRepository
{
    private readonly ApplicationDbContext db;

    public FlowRepository(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<IReadOnlyList<Flow>> GetEnabledFlowsAsync(CancellationToken ct)
    {
        var rows = await this.db.Flows.Where(f => f.IsEnabled).OrderBy(f => f.Order).ToListAsync(ct);
        var links = await this.db.FlowChannels.ToListAsync(ct);

        var linkLookup = links.GroupBy(l => l.FlowId).ToDictionary(g => g.Key, g => g.ToList());
        var flows = new List<Flow>();

        foreach (var r in rows)
        {
            var flow = new Flow(r.Id, r.IsEnabled, r.Name, r.Order);
            if (linkLookup.TryGetValue(r.Id, out var lst))
            {
                foreach (var l in lst)
                {
                    flow.Channels.Add(new FlowChannelLink(l.FlowId, l.ChannelId));
                }
            }
            flows.Add(flow);
        }
        return flows;
    }
}
