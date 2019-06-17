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

namespace EquipmentManagementSystem.Data {


    public class ExportHandler {


        public ExportFile Export(ManagementContext context, Type type, string searchString, string exportType) {

            var file = new ExportFile();

            if (type.Name == "Equipment") {

                var eqpRepo = new EquipmentHandler(context);
                var eqpData = string.IsNullOrEmpty(searchString) ? eqpRepo.GetAll() : eqpRepo.Search(searchString);

                file.FileName = $"EquipmentExport-{DateTime.Now.ToString("dd/MM/yyyy")}";
                file.Data = exportType.ToLower() == "excel" ? ExcelExportReflection(eqpData, typeof(Equipment), new List<string>()) : JsonExport(eqpData);
            }
            else if (type.Name == "Owner") {

                var ownerRepo = new OwnerHandler(context);
                var ownerData = string.IsNullOrEmpty(searchString) ? ownerRepo.GetAll() : ownerRepo.Search(searchString);

                file.FileName = $"OwnerExport-{DateTime.Now.ToString("dd/MM/yyyy")}";
                file.Data = exportType.ToLower() == "excel" ? ExcelExportReflection(ownerData, typeof(Owner), new List<string>() { "FullName", "SSN", "Mail", "TelNr", "Added" }) : JsonExport(ownerData);
            }

            if (exportType.ToLower() == "excel") {

                file.ContentType = "application/excel";
                file.FileName += ".xlsx";
            }
            else {

                file.ContentType = "text/json";
                file.FileName += ".json";
            }

            return file;
        }


        private byte[] JsonExport<T>(IEnumerable<T> data) where T : Entity {


            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return Encoding.ASCII.GetBytes(json);
        }


        private byte[] ExcelExportReflection<T>(IEnumerable<T> data, Type type, List<string> propertyNames) where T : Entity {

            var propertyInfo = GetPropertyInfo(type, propertyNames);

            using (var package = new ExcelPackage()) {

                var typeString = type.ToString().Split(".");

                switch (typeString[typeString.Count() - 1]) {
                    case "Equipment":

                        package.Workbook.Properties.Title = "Equipment";
                        var equipment = SortEquipmentByCategory((IQueryable<Equipment>)data);

                        foreach (var equipType in equipment.Keys) {

                            propertyNames = GetEquipmentPropertyNames(equipType, equipment[equipType][0]);
                            propertyInfo = GetPropertyInfo(type, propertyNames);

                            var eqpsheet = package.Workbook.Worksheets.Add(equipType.ToString());
                            eqpsheet.InsertRow(3, equipment[equipType].Count);
                            eqpsheet = AddPropertyHeader(eqpsheet, propertyNames);

                            for (int i = 0; i < equipment[equipType].Count; i++) {

                                var values = GetPropertyValue(equipment[equipType][i], propertyInfo);
                                
                                eqpsheet = AddPropertyValues(eqpsheet, i + 3, values);
                            }

                            eqpsheet.Cells.AutoFitColumns(0);
                        }
                        break;

                    case "Owner":

                        package.Workbook.Properties.Title = "Owners";

                        var sheet = package.Workbook.Worksheets.Add("Owner");                        

                        var ownerData = data.ToList();

                        sheet.InsertRow(3, ownerData.Count);
                        sheet = AddPropertyHeader(sheet, propertyNames);

                        for (int i = 0; i < ownerData.Count; i++) {

                            var values = GetPropertyValue(ownerData[i], propertyInfo);
                            sheet = AddPropertyValues(sheet, i + 3, values);
                        }

                        sheet.Cells.AutoFitColumns(0);

                        break;
                }

                package.Workbook.Properties.Company = $"Övertorneå Kommun {DateTime.Now.ToString("dd/MM/YYYY")}";

                return package.GetAsByteArray();
            }
        }


        private List<PropertyInfo> GetPropertyInfo(Type type, List<string> propertyNames) {

            var properties = new List<PropertyInfo>();

            for (int i = 0; i < propertyNames.Count; i++) {

                properties.Add(type.GetProperty(propertyNames[i]));
            }

            return properties;
        }


        private List<string> GetPropertyValue<T>(T instance, List<PropertyInfo> properties) where T : Entity {

            return (from val in properties
                    select (val.GetValue(instance) ?? "").ToString())
                   .ToList();
        }



        private ExcelWorksheet AddPropertyHeader(ExcelWorksheet sheet, List<string> propertyNames) {

            // Sets property names as top column name
            for (int i = 0; i < propertyNames.Count; i++) {

                sheet.Cells[1, i + 3].Value = propertyNames[i];
            }

            return sheet;
        }

        private ExcelWorksheet AddPropertyValues(ExcelWorksheet sheet, int row, List<string> values) {

            for (int i = 0; i < values.Count; i++) {

                sheet.Cells[row, i + 3].Value = values[i];
            }

            return sheet;
        }


        private Dictionary<Equipment.EquipmentType, List<Equipment>> SortEquipmentByCategory(IQueryable<Equipment> data) {

            var equipment = new Dictionary<Equipment.EquipmentType, List<Equipment>>();
            var types = Enum.GetValues(typeof(Equipment.EquipmentType));

            foreach (Equipment.EquipmentType equipType in types) {

                var equip = GetEquipment(data, equipType);
                if (equip.Count > 0) {
                    equipment.Add(equipType, equip);
                }
            }
         
            return equipment;
        }

        private List<Equipment> GetEquipment(IQueryable<Equipment> data, Equipment.EquipmentType equipType) {

            return (from m in data
                    where m.EquipType == equipType
                    select m).ToList();
        }


        private List<string> GetEquipmentPropertyNames(Equipment.EquipmentType type, Equipment equipment) {

            var prop = new List<string>();

            prop.Add("LastEdited");
            prop.Add("Model");
            prop.Add("Serial");

            prop.Add("OwnerName");
            prop.Add("Location");

            switch (type) {

                case Equipment.EquipmentType.Projector:
                    prop.Add("Resolution");
                    break;

                case Equipment.EquipmentType.Mobile:
                    prop.Add("Mobile Number");
                    break;
                case Equipment.EquipmentType.Printer:
                    prop.Add("IP");
                    break;
                case Equipment.EquipmentType.Router:
                case Equipment.EquipmentType.Switch:
                    prop.Add("IP");
                    prop.Add("Ports");
                    break;
                default:
                    break;
            }

            prop.Add("Notes");

            return prop;
        }


    }

    public class ExportFile {

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }



}
