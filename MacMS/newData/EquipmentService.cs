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

        internal ManagementContext _context;
        private IValidator _validator;


        public EquipmentService(ManagementContext context, IValidator validator) {

            _context = context;
            _validator = validator;
        }


        public async Task<IEnumerable<Equipment>> GetAll() {

            return await _context.Set<Equipment>().ToListAsync();
        }

        public IQueryable<Equipment> GetAllAsQueryable() {

            return _context.Set<Equipment>().AsQueryable();
        }


        public async Task<IEnumerable<Equipment>> GetAll(Expression<Func<Equipment, bool>> predicate) {

            return await _context.Equipment.Where(predicate).ToListAsync();
        }


        public async Task<IEnumerable<Equipment>> Sort<TKey>(IQueryable<Equipment> query, string sortVariable) {

            switch (sortVariable) {
                case "Date_desc":
                    return await query.OrderByDescending(e => e.LastEdited).ToListAsync();
                case "Date":
                     return await query.OrderBy(e => e.LastEdited).ToListAsync();

                case "Owner_desc":
                    return await query.OrderByDescending(e => e.OwnerName).ToListAsync();
                case "Owner":
                    return await query.OrderBy(e => e.OwnerName).ToListAsync();
                default:
                    return null;
            }
        }


        public async Task<Equipment> GetById(int id) {

            return await _context.Equipment.Where(q => q.ID == id).FirstAsync();
        }


        public async Task<Equipment> FirstOrDefault(Expression<Func<Equipment, bool>> predicate) {

            return await _context.Equipment.FirstOrDefaultAsync(predicate);
        }

        
        public async Task<int> Count() {

            return await _context.Equipment.CountAsync();
        }


        public async Task Create(T entity) {

            _context.Equipment.Add(entity);
            await _context.SaveChangesAsync();
        }


        public async Task Remove(T entity) {

            _context.Equipment.Remove(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> Update(T entity) {

            if (_validator.Validate(entity)) {

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }



        public IQueryable<Equipment> Search(string searchString) {

            var data = GetAllAsQueryable();

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

                            returnData.Concat(from x in data
                                              where x.Model.Contains(query[0])
                                              select x);
                            break;
                        case "Serial":

                            returnData.Concat(from x in data
                                              where x.Serial.Contains(query[0])
                                              select x);
                            break;

                        case "Owner":

                            var ownedEquipment = (from x in _context.Set<Owner>()
                                                 where x.FullName.Contains(query[0])
                                                 select x.Equipment);

                            var test = new List<Equipment>();

                            foreach (var item in ownedEquipment) {
                                test.AddRange(item.ToList());
                            }

                            returnData.Concat(test);
                            break;

                        case "EquipType":

                            Enum.TryParse<Equipment.EquipmentType>(query[1], out var eqpVal);
                            returnData.Concat(from x in data
                                              where x.EquipType == eqpVal
                                              select x);
                            break;

                        default:
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(query[0]) && !string.IsNullOrWhiteSpace(query[0])) {

                    returnData.Concat(from x in data
                                      where x.Model.Contains(query[0]) ||
                                      x.Serial.Contains(query[0]) ||
                                      x.Notes.Contains(query[0]) ||
                                      x.Location.Contains(query[0])
                                      select x);
                }
            }

            return returnData;
        }

    }
}
