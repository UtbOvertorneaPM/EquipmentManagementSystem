using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Business {

    internal static class DataAccessExtension {

        internal static IQueryable<T> AddIncludes<T>(this IQueryable<T> query, params string[] includes) where T : class {

            return includes.Aggregate(query, (current, include) => current.Include(include));
        }
    }
}
