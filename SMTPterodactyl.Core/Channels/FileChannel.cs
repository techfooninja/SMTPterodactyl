namespace SMTPterodactyl.Core.Channels
{
    using MimeKit;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class FileChannel : IChannel
    {
        public string? Name { get; set; }

        public string? FolderPath { get; set; }

        public async Task HandleMessageAsync(MimeMessage message)
        {
            if (string.IsNullOrWhiteSpace(this.FolderPath))
            {
                throw new ArgumentException($"A valid folder path must be specified", nameof(this.FolderPath));
            }

            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }

            await message.WriteToAsync(Path.Combine(this.FolderPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Guid.NewGuid()}.msg"));
        }
    }
}
