using EquipmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EquipmentManagementSystem.newData.Validation;

namespace EquipmentManagementSystem.newData {

    public class EquipmentService<T> where T : Equipment {

        private ManagementContext _context;
        private IValidator<Equipment> _validator;


        public EquipmentService(ManagementContext context, IValidator<Equipment> validator) {

            _context = context;
            _validator = validator;
        }


        public async Task<IEnumerable<Equipment>> GetAll() {

            return await _context.Set<Equipment>().Include(e => e.Owner).ToListAsync();
        }

        public async Task<IEnumerable<Equipment>> GetAll(Expression<Func<Equipment, bool>> predicate) {

            return await _context.Equipment.Where(predicate).ToListAsync();
        }


        public async Task<IEnumerable<Equipment>> Sort<TKey>(IQueryable<Equipment> query, Expression<Func<Equipment, TKey>> order) {

            return await query.OrderBy(order).ToListAsync();
        }


        public async Task<Equipment> GetById(int id) {

            return await _context.Equipment.Where(q => q.ID == id).Include(e => e.Owner).FirstAsync();
        }

        public async Task<Equipment> FirstOrDefault(Expression<Func<Equipment, bool>> predicate) {

            return await _context.Equipment.FirstOrDefaultAsync(predicate);
        }


        public async Task Remove(T entity) {

            _context.Equipment.Remove(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> Update(T entity) {

            if (await _validator.Validate(entity)) {

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }


        private async Task<IEnumerable<Equipment>> Search(string searchString, IQueryable<Equipment> data) {

            var parameter = Expression.Parameter(typeof(Equipment), "type");
            var queryValues = searchString.Split(",");

            var returnData = Enumerable.Empty<Equipment>().AsQueryable();

            for (int i = 0; i < queryValues.Length; i++) {

                var query = queryValues[i].Split(":");

                if (query.Length > 1) {

                    switch (query[0]) {

                        case "LastEdited":

                            var dates = query[1]
                                .Replace("/", " ")
                                .Split(' ');

                            for (int j = 0; j < dates.Count(); j++) {
                                returnData.Concat(from x in data
                                           where x.LastEdited.ToString().Contains(dates[j])
                                           select x);
                            }

                            break;

                        case "Model":
                        case "Serial":


                            queries.Add(Contains(query[0], constant));
                            break;

                        case "Owner":

                            queries.Add(Contains("FullName", constant, true));
                            break;

                        case "EquipType":

                            Enum.TryParse<Equipment.EquipmentType>(query[1], out var eqpVal);
                            queries.Add(SearchEquipmentType(eqpVal, parameter));
                            break;

                        default:
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(query[0]) && !string.IsNullOrWhiteSpace(query[0])) {

                    constant = Expression.Constant(query[0]);
                    queries.AddRange(SearchWide(parameter, constant));
                }
            }

            var eqp = new List<Equipment>();

            for (int i = 0; i < data.Count(); i++) {

                eqp.AddRange(await data.ToListAsync());
            }

            return eqp;
        }

    }
}
