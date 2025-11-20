namespace SMTPterodactyl.Core.Channels;

using System;

public class FileChannel : Channel
{
    public FileChannel(
        Guid id,
        string name,
        string folderPath) : base(id, name)
    {
        FolderPath = folderPath;
    }

    public string FolderPath { get; private set; }
}
