namespace SMTPterodactyl.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDataStore<T>
    {
        Task<T> CreateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<IReadOnlyList<T>> GetAsync();

        Task<T> UpdateAsync(T entity);
    }
}
