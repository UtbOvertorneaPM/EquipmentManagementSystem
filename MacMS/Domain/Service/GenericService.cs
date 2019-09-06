using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service {


    public class GenericService : IGenericService {


        DbContext _context;

        public GenericService(DbContext context) {

            _context = context;
        }

        public IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class {

            return _context.Set<T>().Where(predicate);
        }

        public async Task<T> GetById<T>(int id) where T : class {

            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class {

            return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public IQueryable<T> GetAll<T>() where T : class {

            return _context.Set<T>().AsQueryable();
        }


        public async Task Create<T>(T entity) where T : class {

            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }


        public async Task Update<T>(T entity) where T : class {

            _context.Update(entity);
            await _context.SaveChangesAsync();
        }


        public async Task Remove<T>(T entity) where T : class {

            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task<int> Count<T>() where T : class {

            return await _context.Set<T>().CountAsync();
        }

    }






}
