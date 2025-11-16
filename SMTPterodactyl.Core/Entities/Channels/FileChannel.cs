namespace SMTPterodactyl.Core.Entities.Channels
{
    using MimeKit;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class FileChannel : Channel
    {
        public FileChannel(
            Guid id,
            string name,
            string folderPath) : base(id, name)
        {
            this.FolderPath = folderPath;
        }

        public string FolderPath { get; private set; }

        public async override Task HandleMessageAsync(MimeMessage message)
        {
            if (string.IsNullOrWhiteSpace(FolderPath))
            {
                throw new ArgumentException($"A valid folder path must be specified", nameof(FolderPath));
            }

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            await message.WriteToAsync(Path.Combine(FolderPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Guid.NewGuid()}.msg"));
        }
    }
}
