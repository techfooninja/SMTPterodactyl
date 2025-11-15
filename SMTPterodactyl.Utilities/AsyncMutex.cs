namespace SMTPterodactyl.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An async-compatible mutex using SemaphoreSlim.
    /// </summary>
    public sealed class AsyncMutex : IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IAsyncDisposable> LockAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            return new Releaser(_semaphore);
        }

        public async ValueTask DisposeAsync()
        {
            _semaphore.Dispose();
            await Task.CompletedTask;
        }

        private sealed class Releaser : IAsyncDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private bool _disposed;

            public Releaser(SemaphoreSlim semaphore) => _semaphore = semaphore;

            public async ValueTask DisposeAsync()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _semaphore.Release();
                }

                await Task.CompletedTask;
            }
        }
    }
}
