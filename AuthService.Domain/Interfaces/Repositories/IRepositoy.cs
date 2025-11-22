using AuthService.Domain.Entities.Common;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
}