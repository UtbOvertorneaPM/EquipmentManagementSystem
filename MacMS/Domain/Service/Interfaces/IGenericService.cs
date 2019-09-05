using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service {
    public interface IGenericService<T> where T : class {
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Remove(T entity);
        Task Update(T entity);
    }
}