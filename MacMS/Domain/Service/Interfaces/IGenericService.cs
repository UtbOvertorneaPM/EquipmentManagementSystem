using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service {
    public interface IGenericService {
        Task<int> Count<T>() where T : class;
        Task Create<T>(T entity) where T : class;
        IQueryable<T> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> GetAll<T>() where T : class;
        Task<T> GetById<T>(int id) where T : class;
        Task Remove<T>(T entity) where T : class;
        Task Update<T>(T entity) where T : class;
    }
}