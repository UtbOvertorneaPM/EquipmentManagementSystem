﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using EquipmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
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
                
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                return Encoding.ASCII.GetBytes(json);
            }
            catch (Exception) {

                throw;
            }
        }





        private byte[] ExcelExport(IQueryable<Owner> data) {



        }


        private byte[] ExcelExport(IQueryable<Equipment> data) {


            var macs = (from m in data
                       where m.EquipType == Equipment.EquipmentType.Mac
                       select m).ToList();

            var chromebooks = (from m in data
                               where m.EquipType == Equipment.EquipmentType.Chromebook
                               select m).ToList();

            var mobiles = (from m in data
                           where m.EquipType == Equipment.EquipmentType.Mobile
                           select m).ToList();

            var switches = (from m in data
                           where m.EquipType == Equipment.EquipmentType.Switch
                           select m).ToList();

            var routers = (from m in data
                           where m.EquipType == Equipment.EquipmentType.Router
                           select m).ToList();

            var printers = new List<Equipment>();
            var laptop = new List<Equipment>();
            var projectors = new List<Equipment>();

        }


    }

    public class ExportFile {

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }



}
