using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using EquipmentManagementSystem.Models;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection;
using System.Collections.Concurrent;

namespace EquipmentManagementSystem.Data {


    public class CommandHandler<T> where T : DbContext {


        //protected T context;
        public T context;
        protected const int PAGESIZE = 50;

        public int PageSize { get { return PAGESIZE; } }

        public CommandHandler(T ctx) {

            context = ctx;
        }

       
        public Task Delete<X>(int id) where X : Entity {

            IQueryable<X> obj = context.Set<X>().Where(e => e.ID == id);
            context.Set<X>().RemoveRange(obj);

            return context.SaveChangesAsync();
        }


        public Task<X> Get<X>(int id) where X : Entity {

            return context.Set<X>().FirstOrDefaultAsync(e => e.ID == id);
        }


        public IQueryable<X> GetAll<X>(int page = 0) where X : Entity {

            return context.Set<X>();
        }


        public int Count<X>() where X : Entity {

            return context.Set<X>().Count();
        }


        public IQueryable<X> Search<X>(Expression<Func<X, bool>> search) where X : Entity {
                
            return GetAll<X>().Where(search);
        }

        public IQueryable<X> Search<X>(Expression<Func<X, bool>> search, IQueryable<X> data) where X : Entity {

            return data.Where(search);
        }


        /// <summary>
        /// Handles include searches
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <param name="search"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IQueryable<X> Search<X>(List<Expression<Func<X, bool>>> searchList, IQueryable<X> data) where X : Entity {

            var queryList = new List<IQueryable<X>>();
            
            for (int i = 0; i < searchList.Count; i++) {

                queryList.Add(Search(searchList[i], data));
            }

            return queryList.SelectMany(q => q).Distinct().AsQueryable();
        }


        /// <summary>
        /// Handles searches
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <param name="search"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IQueryable<X> Search<X>(List<Expression<Func<X, bool>>> searchList) where X : Entity {

            var queryList = new List<IQueryable<X>>();

            for (int i = 0; i < searchList.Count; i++) {

                queryList.Add(Search(searchList[i]));
            }

            return queryList.SelectMany(q => q).Distinct().AsQueryable();
        }


        public IQueryable<X> GetSorted<X, U>(IEnumerable<X> query, List<string> propertyName, bool descending = false) where X : Entity {

            var type = typeof(X);

            var parameter = Expression.Parameter(typeof(X), "e");
            var property = GetNestedProperty(propertyName, parameter);
            var exp = Expression.Lambda<Func<X, U>>(property, parameter);

            return descending ? Queryable.OrderByDescending(query.AsQueryable<X>(), exp) : Queryable.OrderBy(query.AsQueryable<X>(), exp);
        }

        
        protected MemberExpression GetNestedProperty(List<string> propertyName, ParameterExpression parameter) {

            var property = Expression.Property(parameter, propertyName[0]);

            for (int i = 1; i < propertyName.Count; i++) {

                property = Expression.Property(property, propertyName[i]);
            }

            return property;
        }


        public async Task<bool> Insert<X>(X obj, bool save = true) where X : Entity {

            if (context.Set<X>().Where(o => o == obj).Count() == 0) {

                await context.Set<X>().AddAsync(obj);

                if (save) { context.SaveChanges(); }
                return true;
            }
            else { return false; }
        }


        public int Update<X>(X obj) where X : Entity {

            context.Set<X>().Update(obj);
            return context.SaveChanges();
        }


        /// <summary>
        /// Saves changes to Db
        /// </summary>
        public void Save() {

            context.SaveChanges();
        }
    }
}
