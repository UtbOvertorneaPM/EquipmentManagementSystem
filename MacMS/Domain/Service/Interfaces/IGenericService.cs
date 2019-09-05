using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service {
    public interface IGenericService<T> where T : class {
        Task Create(T entity);
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        Task<T> GetById(int id);
        Task Remove(T entity);
        Task Update(T entity);
        Task<int> Count();
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
    }
}