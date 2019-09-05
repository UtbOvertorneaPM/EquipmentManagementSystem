using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Service.Export;
using EquipmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Business {

    public class EquipmentRequestHandler<T> where T : class {

        private IGenericService<Equipment> _service;

        public EquipmentRequestHandler(IGenericService<Equipment> service) {

            _service = service;
        }

        public async Task<IEnumerable<Equipment>> Get(Expression<Func<Equipment, bool>> predicate) =>
            await _service.Get(predicate).ToListAsync();

        public async Task<Equipment> GetById(int id) =>
            await _service.GetById(id);

        public async Task Create(Equipment equipment) =>
            await _service.Create(equipment);

        public async Task Remove(Equipment equipment) =>
            await _service.Remove(equipment);

        public async Task Update(Equipment equipment) =>    
            await _service.Update(equipment);

        public async Task<Equipment> FirstOrDefault(Expression<Func<Equipment, bool>> predicate) =>
            await _service.FirstOrDefault(predicate);


        public async Task<FileStreamResult> Export(string searchString, string selection, ExportType exportType) {

            ExportFile file;
            var data = Enumerable.Empty<Equipment>();

            if (string.IsNullOrEmpty(selection)) {
                if (searchString == "Find model/date/owner...") {
                    data = await _service.GetAll().AddIncludes(DataIncludes.Owner).ToListAsync();
                }
                else {
                    data = await EquipmentDataFormatting.Search(_service.GetAll(), searchString);
                }                
            }
            else {

                var serials = selection.Trim().Replace("\n", " ").Split(" ");

                for (int i = 0; i < serials.Count(); i++) {

                    data.Concat(_service.Get(x => x.Serial == serials[i]).AddIncludes(DataIncludes.Owner));
                }

            }


            file = await new ExportService<Equipment>().Export(data, exportType);
            return new FileStreamResult(new MemoryStream(file.Data), file.ContentType) { FileDownloadName = file.FileName };

        }


        public async Task<PagedList<Equipment>> IndexRequest(string sortVariable, string searchString, int page, int pageSize) {

            var pagedList = new PagedList<Equipment>();

            var data = Enumerable.Empty<Equipment>();
            var query = Enumerable.Empty<Equipment>();
            var searched = false;

            // Search
            if (!string.IsNullOrEmpty(searchString)) {

                query = await EquipmentDataFormatting.Search(_service.GetAll(), searchString);
                searched = true;
            }

            // Sort
            if (!string.IsNullOrEmpty(sortVariable)) {

                if (searched) {

                    data = EquipmentDataFormatting.Sort<Equipment>(query, sortVariable);
                }
                else {

                    data = EquipmentDataFormatting.Sort<Equipment>(_service.GetAll(), sortVariable);
                }
                
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), await _service.Count(), page, pageSize);

            }
            // Index request without modifiers
            else {

                data = EquipmentDataFormatting.Sort<Equipment>(_service.GetAll(), "Date_desc");
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), await _service.Count(), page, pageSize);
            }


            return pagedList;
        }
    }
}
