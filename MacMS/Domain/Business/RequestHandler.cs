using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Service.Export;
using EquipmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static EquipmentManagementSystem.Domain.Service.Export.ExportService;

namespace EquipmentManagementSystem.Domain.Business {

    public class RequestHandler<T> where T : class {

        IGenericService<Equipment> _service;

        public RequestHandler(IGenericService<Equipment> service) {

            _service = service;
        }



        public async Task<FileStreamResult> Export(string searchString, string selection, ExportType exportType) {

            var data = Enumerable.Empty<Equipment>();
            ExportFile file;

            if (string.IsNullOrEmpty(selection)) {

                data = await _service.GetAll();
            }
            else {

                var serials = selection.Trim().Replace("\n", " ").Split(" ");

                for (int i = 0; i < serials.Count(); i++) {

                    data.Concat(_service.Get(x => x.Serial == serials[i]));
                }
            }

            file = await new ExportService().Export(data, exportType);
            return new FileStreamResult(new MemoryStream(file.Data), file.ContentType) { FileDownloadName = file.FileName };

        }
    }
}
