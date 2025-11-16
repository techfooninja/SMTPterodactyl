namespace SMTPterodactyl.Core.Interfaces.Repositories
{
    using SMTPterodactyl.Core.Entities;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository<T> : IDisposable where T : Entity
    {
        Task AddAsync(T entity);

        Task DeleteAsync(T entity);

        Task<IQueryable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(Guid id);

        Task<int> SaveChangesAsync();

        Task UpdateAsync(T entity);
    }
}
