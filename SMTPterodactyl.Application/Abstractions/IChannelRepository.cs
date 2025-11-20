namespace SMTPterodactyl.Application.Abstractions;

using SMTPterodactyl.Core.Channels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IChannelRepository
{
    Task<Channel?> GetByIdAsync(Guid channelId, CancellationToken ct);

    Task<IReadOnlyDictionary<Guid, Channel>> GetByIdsAsync(IEnumerable<Guid> channelIds, CancellationToken ct);
}