namespace SMTPterodactyl.Application.Abstractions;

using SMTPterodactyl.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFlowRepository
{
    Task<IReadOnlyList<Flow>> GetEnabledFlowsAsync(CancellationToken ct);
}
