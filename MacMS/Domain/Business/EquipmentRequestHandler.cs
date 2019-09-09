using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Domain.Data;
using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Service.Export;
using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.newData.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static EquipmentManagementSystem.Domain.Data.Models.Equipment;

namespace EquipmentManagementSystem.Domain.Business {

    public class EquipmentRequestHandler {

        private IGenericService _service;
        private IValidator _validator;

        public EquipmentRequestHandler(IGenericService service, IValidator validator) {

            _service = service;
            _validator = validator;
        }

        public IQueryable<T> GetAll<T>() where T : class =>
            _service.GetAll<T>();

        public IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class =>
            _service.Get(predicate);

        public async Task<T> GetById<T>(int id) where T : Equipment  =>
            await _service.Get<T>(e => e.ID == id).FirstOrDefaultAsync();

        public async Task<bool> Create<T>(T equipment) where T : class {

            if (_validator.Validate<T>(equipment)) {

                await _service.Create(equipment);
                return true;
            }

            return false;
        }
            
            

        public async Task<bool> Remove<T>(T equipment) where T : class {

            if (_validator.Validate<T>(equipment)) {

                await _service.Remove(equipment);
                return true;
            }

            return false;
        }
            

        public async Task<bool> Update<T>(T equipment) where T : class {

            if (_validator.Validate(equipment)) {

                await _service.Update(equipment);
                return true;
            }

            return false;            
        }   
            

        public IQueryable<T> FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class =>
            _service.FirstOrDefault(predicate);



        public async Task<FileStreamResult> Export(string searchString, string selection, ExportType exportType) {

            ExportFile file;
            var data = Enumerable.Empty<Equipment>();

            if (string.IsNullOrEmpty(selection)) {

                if (searchString == "Find model/date/owner...") {

                    data = await _service.GetAll<Equipment>().ToListAsync();
                }
                else {

                    data = await EquipmentDataFormatting.Search(_service.GetAll<Equipment>(), searchString);
                }                
            }
            else {

                var minSerialLength = 5;

                var serials = selection.Trim().Replace("\n", " ").Split(" ");
                serials = serials.Where(s => !string.IsNullOrWhiteSpace(s) && s.Length > minSerialLength).Distinct().ToArray();

                List<Equipment> temp = new List<Equipment>();
                for (int i = 0; i < serials.Count(); i++) {

                    temp.Add(await _service.FirstOrDefault<Equipment>(x => x.Serial == serials[i]).FirstAsync());                    
                }
                data = temp;
            }

            file = await new ExportService<Equipment>().Export(data, exportType);
            return new FileStreamResult(new MemoryStream(file.Data), file.ContentType) { FileDownloadName = file.FileName };
        }




        public async Task<PagedList<EquipmentViewModel>> IndexRequest<T>(string sortVariable, string searchString, int page, int pageSize, string type = null) where T : class {

            var pagedList = new PagedList<EquipmentViewModel>();
            var list = new List<EquipmentViewModel>();
            int count;

            if (string.IsNullOrEmpty(type) is false) {

                Enum.TryParse(type, out EquipmentType choiceType);
                count = await _service.Count<Equipment>(x => x.EquipType == choiceType);
            }
            else {

                count = await _service.Count<Equipment>();
            }
            

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

                foreach (var item in data) {

                    list.Add(new EquipmentViewModel() { Equipment = item });
                }

                pagedList.Initialize(list.Skip(page * pageSize).Take(pageSize), count, page, pageSize);
            }
            // Index request without modifiers
            else {

                data = EquipmentDataFormatting.Sort<Equipment>(_service.GetAll<Equipment>(), "Date_desc");

                foreach (var item in data) {

                    list.Add(new EquipmentViewModel() { Equipment = item });
                }

                pagedList.Initialize(list.Skip(page * pageSize).Take(pageSize), count, page, pageSize);
            }

            return pagedList;
        }


        public async Task DeleteSelection(string serial) {

            var minSerialLength = 5;

            var serials = serial.Trim().Replace("\n", " ").Split(" ");
            serials = serials.Where(s => !string.IsNullOrWhiteSpace(s) && s.Length > minSerialLength).Distinct().ToArray();

            for (int i = 0; i < serials.Count(); i++) {

                if (string.IsNullOrWhiteSpace(serials[i]) is false) {

                    await _service.Remove(await _service.FirstOrDefault<Equipment>(e => e.Serial == serials[i]).FirstOrDefaultAsync());
                }
            }
        }
    }
}
