using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfficeOpenXml;
using OfficeOpenXml.Style;
using EquipmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
using EquipmentManagementSystem.newData;
using EquipmentManagementSystem.Data.Export;

namespace EquipmentManagementSystem.Data {


    public static class ExportHandler {


        public static async Task<ExportFile> Export<T>(IEnumerable<T> data, Type type, string searchString, string exportType, string selection) where T : class {

            var file = new ExportFile();
            IExporter exporter;

            if (exportType.ToLower() == "excel") {

                file.ContentType = "application/excel";
                file.FileSuffix += ".xlsx";
                exporter = new ExcelExporter();
            }
            else {

                file.ContentType = "text/json";
                file.FileSuffix += ".json";
                exporter = new JsonExporter();
            }

            if (type.Name == "Equipment") {

                file.FileName = $"EquipmentExport-{DateTime.Now.ToString("dd/MM/yyyy")}";
                file.Data = await exporter.Export((IEnumerable<Equipment>)data);

            }
            else if (type.Name == "Owner") {


                file.FileName = $"OwnerExport-{DateTime.Now.ToString("dd/MM/yyyy")}";
                file.Data = await exporter.Export((IEnumerable<Owner>)data);
            }

            return file;

        }


    }



}

