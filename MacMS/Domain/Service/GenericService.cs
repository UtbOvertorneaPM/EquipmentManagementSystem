using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service {


    public class GenericService<T> : IGenericService<T> where T : class {


        DbContext _context;

        public GenericService(DbContext context) {

            _context = context;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate) {

            return _context.Set<T>().Where(predicate);
        }

        public async Task<T> GetById(int id) {

            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate) {

            return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public IQueryable<T> GetAll() {

            return _context.Set<T>().AsQueryable();
        }


        public async Task Create(T entity) {

            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }


        public async Task Update(T entity) {

            _context.Update(entity);
            await _context.SaveChangesAsync();
        }


        public async Task Remove(T entity) {

            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task<int> Count() {

            return await _context.Set<T>().CountAsync();
        }
    }






}
