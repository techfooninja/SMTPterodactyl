namespace SMTPterodactyl.Core.Messages;

using System;
using System.Collections.Generic;

public sealed record MessageEnvelope(
    string MessageId,
    string From,
    IReadOnlyList<string> To,
    string? Subject,
    string? PlainTextBody,
    string? HtmlBody,
    DateTimeOffset ReceivedAtUtc,
    IReadOnlyList<MessageMedia> Media,
    IReadOnlyDictionary<string, string> Headers);
