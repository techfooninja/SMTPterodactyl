namespace SMTPterodactyl.Application.UseCases;

using SMTPterodactyl.Application.Abstractions;
using SMTPterodactyl.Application.Services.FlowMatcher;
using SMTPterodactyl.Core.Channels;
using SMTPterodactyl.Core.Messages;
using System.Linq;
using System.Threading.Tasks;

public sealed record ProcessInboundMessageCommand(MessageEnvelope Envelope);

public class ProcessInboundMessageHandler
{
    private readonly IFlowRepository flows;
    private readonly IChannelRepository channels;
    private readonly IFlowMatcher matcher;
    private readonly ITelegramService telegram;

    public ProcessInboundMessageHandler(
        IFlowRepository flows,
        IChannelRepository channels,
        IFlowMatcher matcher,
        ITelegramService telegramService)
    {
        this.flows = flows;
        this.channels = channels;
        this.matcher = matcher;
        this.telegram = telegramService;
    }

    public async Task HandleAsync(ProcessInboundMessageCommand command, CancellationToken ct = default)
    {
        var enabledFlows = await this.flows.GetEnabledFlowsAsync(ct);
        var matchedFlows = this.matcher.Match(command.Envelope, enabledFlows);

        var endpointIds = matchedFlows.SelectMany(f => f.Channels).Select(l => l.EndpointId).Distinct().ToList();
        var channels = await this.channels.GetByIdsAsync(endpointIds, ct);

        foreach (var flow in matchedFlows)
        {
            foreach (var link in flow.Channels)
            {
                if (!channels.TryGetValue(link.EndpointId, out var channel) || !channel.IsActive) continue;

                switch (channel)
                {
                    // TODO: Add the other channels
                    case TelegramChannel t:
                        await this.telegram.SendAsync(t.BotConfiguration.BotToken, t.ChatId, command.Envelope);
                        break;
                }
            }
        }
    }
}