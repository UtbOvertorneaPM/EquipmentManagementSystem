﻿using EquipmentManagementSystem.Data;
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

    public class EquipmentRequestHandler {

        private IGenericService _service;

        public EquipmentRequestHandler(IGenericService service) {

            _service = service;
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : class =>
            await _service.GetAll<T>().ToListAsync();

        public async Task<IEnumerable<T>> Get<T>(Expression<Func<T, bool>> predicate) where T : class =>
            await _service.Get(predicate).ToListAsync();

        public async Task<T> GetById<T>(int id) where T : class  =>
            await _service.GetById<T>(id);

        public async Task Create<T>(T equipment) where T : class =>
            await _service.Create<T>(equipment);
            

        public async Task Remove<T>(T equipment) where T : class =>
            await _service.Remove(equipment);

        public async Task Update<T>(T equipment) where T : class =>    
            await _service.Update(equipment);

        public async Task<T> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class =>
            await _service.FirstOrDefault(predicate);



        public async Task<FileStreamResult> Export(string searchString, string selection, ExportType exportType) {

            ExportFile file;
            var data = Enumerable.Empty<Equipment>();

            if (string.IsNullOrEmpty(selection)) {
                if (searchString == "Find model/date/owner...") {
                    data = await _service.GetAll<Equipment>().AddIncludes(DataIncludes.Owner).ToListAsync();
                }
                else {
                    data = await EquipmentDataFormatting.Search(_service.GetAll<Equipment>(), searchString);
                }                
            }
            else {

                var serials = selection.Trim().Replace("\n", " ").Split(" ");

                for (int i = 0; i < serials.Count(); i++) {

                    data.Concat(_service.Get<Equipment>(x => x.Serial == serials[i]).AddIncludes(DataIncludes.Owner));
                }

            }


            file = await new ExportService<Equipment>().Export(data, exportType);
            return new FileStreamResult(new MemoryStream(file.Data), file.ContentType) { FileDownloadName = file.FileName };

        }


        public async Task<PagedList<Equipment>> IndexRequest<T>(string sortVariable, string searchString, int page, int pageSize) where T : class {

            var pagedList = new PagedList<Equipment>();

            var data = Enumerable.Empty<Equipment>();
            var query = Enumerable.Empty<Equipment>();
            var searched = false;

            // Search
            if (!string.IsNullOrEmpty(searchString)) {

                query = await EquipmentDataFormatting.Search(_service.GetAll<Equipment>(), searchString);
                searched = true;
            }

            // Sort
            if (!string.IsNullOrEmpty(sortVariable)) {

                if (searched) {

                    data = EquipmentDataFormatting.Sort<Equipment>(query, sortVariable);
                }
                else {

                    data = EquipmentDataFormatting.Sort<Equipment>(_service.GetAll<Equipment>(), sortVariable);
                }
                
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), await _service.Count<T>(), page, pageSize);

            }
            // Index request without modifiers
            else {

                data = EquipmentDataFormatting.Sort<Equipment>(_service.GetAll<Equipment>(), "Date_desc");
                pagedList.Initialize(data.Skip(page * pageSize).Take(pageSize), await _service.Count<T>(), page, pageSize);
            }


            return pagedList;
        }
    }
}
