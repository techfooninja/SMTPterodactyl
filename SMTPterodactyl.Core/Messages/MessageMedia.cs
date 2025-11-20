namespace SMTPterodactyl.Core.Messages;

public sealed record MessageMedia(
    string FileName,
    string ContentType,
    byte[] Content);
