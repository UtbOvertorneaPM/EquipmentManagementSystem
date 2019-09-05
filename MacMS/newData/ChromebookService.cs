using EquipmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EquipmentManagementSystem.newData.Validation;
using System.IO;
using EquipmentManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using EquipmentManagementSystem.Data.Export;

namespace EquipmentManagementSystem.newData {

    public class ChromebookService<T> where T : Equipment {
        /*
        internal ManagementContext _context;
        private IValidator<Equipment> _validator;

        public ChromebookService(ManagementContext context, IValidator<Equipment> validator) {

            _context = context;
            _validator = validator;
        }


        public async Task<PagedList<Equipment>> IndexRequest(string sortVariable, string searchString, int page, int pageSize) {

            var pagedList = new PagedList<Equipment>();

            var data = Enumerable.Empty<Equipment>();
            var query = Enumerable.Empty<Equipment>().AsQueryable();

            // Search then sort
            if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(sortVariable)) {

                query = Search(searchString += ",EquipType:Chromebook");
                data = await Sort<Equipment>(query, sortVariable);

                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), data.Count(), page, pageSize);
            }
            // Search
            else if (!string.IsNullOrEmpty(searchString)) {

                query = Search(searchString += ",EquipType:Chromebook");
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), data.Count(), page, pageSize);
            }
            // Sort
            else if (!string.IsNullOrEmpty(sortVariable)) {

                data = await Sort<Equipment>(GetAllAsQueryable(), sortVariable);
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), await Count(), page, pageSize);

            }
            // Index request without modifiers
            else {

                data = await Sort<Equipment>(GetAllAsQueryable(), "Date_desc");
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), await Count(), page, pageSize);
            }

            return pagedList;
        }


        public async Task<IEnumerable<Equipment>> GetAll() {

            return await _context.Set<Equipment>().Where(e => e.EquipType == Equipment.EquipmentType.Chromebook).Include(e => e.Owner).ToListAsync();
        }

        public IQueryable<Equipment> GetAllAsQueryable() {

            return _context.Set<Equipment>().Where(e => e.EquipType == Equipment.EquipmentType.Chromebook).Include(e => e.Owner).AsQueryable();
        }


        public async Task<IQueryable<Equipment>> GetAll(Expression<Func<Equipment, bool>> predicate) {

            return await _context.Equipment.Where(predicate).ToListAsync();
        }


        public async Task<Equipment> GetById(int id) {

            return await _context.Equipment.Where(q => q.ID == id && q.EquipType == Equipment.EquipmentType.Chromebook).Include(e => e.Owner).FirstAsync();
        }


        public async Task<Equipment> FirstOrDefault(Expression<Func<Equipment, bool>> predicate) {

            return await _context.Equipment.FirstOrDefaultAsync(predicate);
        }


        public async Task<int> Count() {

            return await _context.Equipment.Where(e => e.EquipType == Equipment.EquipmentType.Chromebook).CountAsync();
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

            if (await _validator.Validate(entity)) {

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }



       


        public async Task<FileStreamResult> Export(string searchString, string selection, string exportType) {

            switch (exportType) {

                case "excel":
                    _exporter.SetExporter(new ExcelExporter());
                    break;

                case "json":
                    _exporter.SetExporter(new JsonExporter());
                    break;

                default:
                    break;
            }

            var data = await GetExportData(searchString, selection);
            var file = await _exporter.Export(data, typeof(Equipment));

            return new FileStreamResult(new MemoryStream(file.Data), file.ContentType) { FileDownloadName = file.FileName };
        }


        public async Task<IEnumerable<Equipment>> GetExportData(string searchString, string selection) {

            if (string.IsNullOrEmpty(selection)) {

                return string.IsNullOrEmpty(searchString) ? await GetAll() : await Search(searchString).ToListAsync();
            }
            else {

                var serials = selection.Trim().Replace("\n", " ").Split(" ");
                var eqpDataS = new List<Equipment>();

                for (int i = 0; i < serials.Count(); i++) {

                    serials[i] = serials[i].Insert(0, "Serial:");
                }

                return await Search(string.Join("", serials)).ToListAsync();
            }
        }
*/
    }

}
