namespace SMTPterodactyl.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using SMTPterodactyl.Core.Entities;
    using SMTPterodactyl.Core.Interfaces.Repositories;
    using SMTPterodactyl.Infrastructure.Database;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class BaseRepository<T> : IRepository<T> where T : Entity
    {
        private readonly ApplicationDbContext dbContext;
        protected readonly DbSet<T> dbSet;

        protected BaseRepository(ApplicationDbContext context)
        {
            this.dbContext = context;
            this.dbSet = this.dbContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await this.dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            this.dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<IQueryable<T>> GetAllAsync()
        {
            return Task.FromResult(this.dbSet.AsQueryable());
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await this.dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            this.dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.dbContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dbContext.Dispose();
            }
        }
    }
}
