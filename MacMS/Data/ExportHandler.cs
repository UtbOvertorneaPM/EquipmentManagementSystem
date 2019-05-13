using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EquipmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EquipmentManagementSystem.Data {


    public class ExportHandler {


        public ExportFile Export(EquipmentHandler repo, string searchString, string exportType) {

            var data = Enumerable.Empty<Equipment>().AsQueryable();
            data = string.IsNullOrEmpty(searchString) ? repo.GetAll() : repo.Search(searchString);

            var file = new ExportFile();
            file.FileName = $"\\EquipmentExport-{DateTime.Now.ToString("dd/MM/YYYY")}";

            switch (exportType.ToLower()) {
                case "excel":

                    throw new NotImplementedException();
                    file.ContentType = "application/excel";
                    file.FileName += ".exl";
                    break;

                case "json":

                    file.ContentType = "text/json";
                    file.FileName += ".json";
                    file.Data = JsonExport<Equipment>(data);
                    break;

                default:

                    throw new Exception();
            }

            return file;
        }


        public ExportFile Export(OwnerHandler repo, string searchString, string exportType) {

            var data = Enumerable.Empty<Owner>().AsQueryable();
            data = string.IsNullOrEmpty(searchString) ? repo.GetAll() : repo.Search(searchString);

            var file = new ExportFile();
            file.FileName = $"\\OwnerExport-{DateTime.Now.ToString("dd/MM/YYYY")}";

            switch (exportType) {
                case "excel":

                    throw new NotImplementedException();
                    file.ContentType = "application/excel";
                    break;

                case "json":

                    file.ContentType = "text/json";
                    file.Data = JsonExport<Owner>(data);
                    break;
                    
                default:

                    throw new Exception();
            }

            return file;
        }


        private byte[] JsonExport<T>(IQueryable<T> data) where T : Entity {

            try {
                
                var json = JsonConvert.SerializeObject(data);
                return Encoding.ASCII.GetBytes(json);
            }
            catch (Exception) {

                throw;
            }
        }
    }

    public class ExportFile {

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }



}
