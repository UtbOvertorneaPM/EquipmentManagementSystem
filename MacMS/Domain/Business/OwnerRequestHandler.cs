using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data;
using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Domain.Data.Validation;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Service.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OwnerManagementSystem.Domain.Business {

    public class OwnerRequestHandler : EquipmentManagementSystem.Domain.Business.IRequestHandler {


        private IGenericService _service;
        private IValidator _validator;


        public OwnerRequestHandler(IGenericService service) {

            _service = service;
            _validator = new Validator(this);
        }

        public IQueryable<T> GetAll<T>() where T : class =>
            _service.GetAll<T>();


        public IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class =>
            _service.Get(predicate);


        public async Task<T> GetById<T>(int id) where T : Owner =>
            await _service.Get<T>(e => e.ID == id).FirstOrDefaultAsync();


        public async Task<bool> Create<T>(T Owner) where T : class {

            if (await _validator.Validate<T>(Owner)) {

                await _service.Create(Owner);
                return true;
            }

            return false;
        }


        public async Task<bool> Remove<T>(T Owner) where T : class {

            if (await _validator.Validate<T>(Owner)) {

                await _service.Remove(Owner);
                return true;
            }

            return false;
        }


        public async Task<bool> Update<T>(T Owner) where T : class {

            if (await _validator.Validate(Owner)) {

                await _service.Update(Owner);
                return true;
            }

            return false;
        }


        public IQueryable<T> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class =>
            _service.FirstOrDefault(predicate);


        public async Task<FileStreamResult> Export(string searchString, string selection, ExportType exportType) {

            ExportFile file;
            var data = Enumerable.Empty<Owner>();

            if (string.IsNullOrEmpty(selection)) {

                if (searchString == "Find model/date/owner...") {

                    data = await _service.GetAll<Owner>().ToListAsync();
                }
                else {

                    data = await OwnerDataFormatting.Search(_service.GetAll<Owner>(), searchString);
                }
            }
            else {

                var minSerialLength = 5;

                var names = selection.Trim().Replace("\n", " ").Split(" ");
                names = names.Where(s => !string.IsNullOrWhiteSpace(s) && s.Length > minSerialLength).Distinct().ToArray();

                List<Owner> temp = new List<Owner>();
                for (int i = 0; i < names.Count(); i++) {

                    temp.Add(await _service.FirstOrDefault<Owner>(x => x.FullName == names[i]).FirstAsync());
                }

                data = temp;
            }

            file = await new ExportService<Owner>().Export(data, exportType);
            return new FileStreamResult(new MemoryStream(file.Data), file.ContentType) { FileDownloadName = file.FileName };
        }


        public async Task<PagedList<Owner>> IndexRequest<T>(IndexRequestModel request) where T : class {

            var pagedList = new PagedList<Owner>();
            var list = new List<Owner>();
            int count = await _service.Count<Owner>(); ;


            var data = Enumerable.Empty<Owner>();
            var query = Enumerable.Empty<Owner>();
            var searched = false;

            // Search
            if (!string.IsNullOrEmpty(request.SearchString)) {

                query = await OwnerDataFormatting.Search(_service.GetAll<Owner>(), request.SearchString);
                searched = true;
            }

            // Sort
            if (!string.IsNullOrEmpty(request.SortVariable)) {

                if (searched) {

                    data = OwnerDataFormatting.Sort<Owner>(query, request.SortVariable);
                }
                else {

                    data = OwnerDataFormatting.Sort<Owner>(_service.GetAll<Owner>(), request.SortVariable);
                }

                foreach (var item in data) {

                    list.Add(item);
                }

                pagedList.Initialize(list.Skip(request.Page * request.PageSize).Take(request.PageSize), request, count);
            }
            // Index request without modifiers
            else {

                data = OwnerDataFormatting.Sort<Owner>(_service.GetAll<Owner>(), "Date_desc");

                foreach (var item in data) {

                    list.Add(item);
                }

                pagedList.Initialize(list.Skip(request.Page * request.PageSize).Take(request.PageSize), request, count);
            }

            return pagedList;
        }


        public async Task DeleteSelection(string name) {

            var minSerialLength = 5;

            var names = name.Trim().Replace("\n", " ").Split(" ");
            names = names.Where(s => !string.IsNullOrWhiteSpace(s) && s.Length > minSerialLength).Distinct().ToArray();

            for (int i = 0; i < names.Count(); i++) {

                if (string.IsNullOrWhiteSpace(names[i]) is false) {

                    await _service.Remove(await _service.FirstOrDefault<Owner>(e => e.FullName == names[i]).FirstOrDefaultAsync());
                }
            }
        }

    }
}
