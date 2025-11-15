namespace SMTPterodactyl.Persistence.Json
{
    using SMTPterodactyl.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class JsonDataStore<T> : IDataStore<T>, IAsyncDisposable
    {
        private readonly string filePath;
        private readonly JsonSerializerOptions options;
        private readonly AsyncMutex mutex = new AsyncMutex();
        private readonly List<T> entities = new List<T>();
        private bool isLoaded;

        public JsonDataStore(string filePath)
        {
            this.filePath = filePath;
            this.options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            this.options.Converters.Add(new PolymorphicConverter<T>());
        }

        public async Task<T> CreateAsync(T entity)
        {
            await using var handle = await this.mutex.LockAsync();
            await this.Load();
            this.entities.Add(entity);
            await this.Save();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            await using var handle = await this.mutex.LockAsync();
            await this.Load();
            this.entities.Remove(entity);
            await this.Save();
        }

        public async ValueTask DisposeAsync()
        {
            await this.mutex.DisposeAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync()
        {
            await using var handle = await this.mutex.LockAsync();
            await this.Load();
            return this.entities.AsReadOnly();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await using var handle = await this.mutex.LockAsync();
            await this.Load();

            // NOTE: There's nothing to do with Update, assuming the caller updated an instance they got from GetAsync().
            //       So we just need to flush the current entities to file.
            await this.Save();
            return entity;
        }

        private async Task Load()
        {
            // NOTE: Load() should always be called from within a critical section
            if (!this.isLoaded)
            {
                try
                {
                    if (!File.Exists(this.filePath))
                    {
                        // TODO: Log something about how the file doesn't exist
                        return;
                    }

                    var json = await File.ReadAllTextAsync(this.filePath);
                    this.entities.AddRange(JsonSerializer.Deserialize<List<T>>(json, this.options) ?? Enumerable.Empty<T>());
                }
                catch (Exception)
                {
                    // TODO: Log something here after we get logging figured out
                    return;
                }
                finally
                {
                    this.isLoaded = true;
                }
            }
        }

        private async Task Save()
        {
            // NOTE: Save() should always be called from within a critical section, and after a call to Load()
            try
            {
                var json = JsonSerializer.Serialize(this.entities, this.options);
                var tempFilePath = this.filePath + ".tmp";

                await File.WriteAllTextAsync(tempFilePath, json);
                File.Move(tempFilePath, this.filePath, true);
            }
            catch (Exception)
            {
                // TODO: Log something here after we get logging figured out
                return;
            }
        }
    }
}
