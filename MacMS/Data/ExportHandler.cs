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
                file.Data = exportType.ToLower() == "excel" ? ExcelExportReflection(ownerData, typeof(Owner), new List<string>() { "Name", "SSN", "Mail", "TelNr", "Added" }) : JsonExport(ownerData);
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


        public ExportFile Export(EquipmentHandler repo, string searchString, string exportType) {

            var data = Enumerable.Empty<Equipment>().AsQueryable();
            data = string.IsNullOrEmpty(searchString) ? repo.GetAll() : repo.Search(searchString);

            var file = new ExportFile();
            file.FileName = $"\\EquipmentExport-{DateTime.Now.ToString("dd/MM/YYYY")}";

            switch (exportType.ToLower()) {
                case "excel":

                    file.ContentType = "application/excel";
                    file.FileName += ".xlsx";
                    file.Data = ExcelExport(data);
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


            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            return Encoding.ASCII.GetBytes(json);
        }


        private byte[] ExcelExportReflection<T>(IQueryable<T> data, Type type, List<string> propertyNames) where T : Entity {

            var propertyInfo = GetPropertyInfo(type, propertyNames);

            using (var package = new ExcelPackage()) {

                //var sheets = new List<ExcelWorksheet>();
                var typeString = type.ToString().Split(".");


                switch (typeString[typeString.Count() - 1]) {
                    case "Equipment":

                        package.Workbook.Properties.Title = "Equipment";
                        var equipment = SortEquipmentByCategory((IQueryable<Equipment>)data);

                        foreach (var equipType in equipment.Keys) {

                            propertyNames = GetEquipmentPropertyNames(equipType, equipment[equipType][0]);

                            propertyInfo = GetPropertyInfo(type, propertyNames);
                            var eqpsheet = package.Workbook.Worksheets.Add(equipType.ToString());
                            
                            eqpsheet = AddPropertyHeader(eqpsheet, propertyNames);

                            for (int i = 0; i < equipment[equipType].Count; i++) {

                                var values = GetPropertyValue(equipment[equipType][i], propertyInfo);
                                eqpsheet.InsertRow(3, 3);
                                eqpsheet = AddPropertyValues(eqpsheet, i + 3, values);
                            }

                            eqpsheet.Cells.AutoFitColumns(0);

                            //sheets.Add(sheet);
                        }
                        break;

                    case "Owner":

                        package.Workbook.Properties.Title = "Owners";

                        var sheet = package.Workbook.Worksheets.Add("Owner");
                        sheet = AddPropertyHeader(sheet, propertyNames);

                        var ownerData = data.ToList();
                        for (int i = 0; i < data.Count(); i++) {

                            var values = GetPropertyValue(ownerData[i], propertyInfo);
                            sheet = AddPropertyValues(sheet, i, values);
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

                sheet.Cells[1, i + 1].Value = propertyNames[i];
            }

            return sheet;
        }

        private ExcelWorksheet AddPropertyValues(ExcelWorksheet sheet, int row, List<string> values) {

            for (int i = 0; i < values.Count; i++) {

                sheet.Cells[row, i + 3].Value = values[i];
            }

            return sheet;
        }





        private byte[] ExcelExport(IQueryable<Owner> data) {


            var owners = data.ToList();

            using (var package = new ExcelPackage()) {

                var sheet = package.Workbook.Worksheets.Add("Owners");


                var propertyNames = new List<string>() { "Name", "SSN", "Mail", "TelNr", "Added" };

                for (int i = 0; i < propertyNames.Count; i++) {

                    sheet.Cells[1, i + 1].Value = propertyNames[i];
                }

                for (int i = 0; i < owners.Count(); i++) {

                    sheet.Cells[i + 3, 1].Value = owners[i].FullName;
                    sheet.Cells[i + 3, 2].Value = owners[i].SSN;
                    sheet.Cells[i + 3, 3].Value = owners[i].Mail;
                    sheet.Cells[i + 3, 4].Value = owners[i].TelNr;
                    sheet.Cells[i + 3, 5].Value = owners[i].Added;
                }

                using (var range = sheet.Cells[1, 5]) {

                    range.Style.Font.Bold = true;
                }

                sheet.Cells.AutoFitColumns(0);

                package.Workbook.Properties.Title = "Owners";
                package.Workbook.Properties.Company = $"Övertorneå Kommun {DateTime.Now.ToString("dd/MM/YYYY")}";

                return package.GetAsByteArray();
            }
        }







        private byte[] ExcelExport(IQueryable<Equipment> data) {

            var equipment = SortEquipmentByCategory(data);

            using (var package = new ExcelPackage()) {

                // Each type of Equipment, create a new sheet
                foreach (var equipType in equipment.Keys) {

                    var sheet = package.Workbook.Worksheets.Add(equipType.ToString());

                    // Get Property names
                    var propertyNames = GetEquipmentPropertyNames(equipType, equipment[equipType][0]);

                    // Sets property names as top column name
                    for (int i = 0; i < propertyNames.Count; i++) {

                        sheet.Cells[1, i + 1].Value = propertyNames[i];
                    }

                    var propertyValues = GetEquipmentPropertyValue(equipType, equipment[equipType]);

                    for (int i = 0; i < propertyValues.Count; i++) {

                        for (int j = 0; j < propertyValues[i].Count; j++) {

                            sheet.Cells[(i + 3), (j + 1)].Value = propertyValues[i][j];
                        }
                    }

                    using (var range = sheet.Cells[1, equipment[equipType].Count]) {

                        range.Style.Font.Bold = true;

                    }

                    sheet.Cells.AutoFitColumns(0);


                }

                package.Workbook.Properties.Title = "Equipment";
                package.Workbook.Properties.Company = $"Övertorneå Kommun {DateTime.Now.ToString("dd/MM/YYYY")}";

                return package.GetAsByteArray();
            }
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

            prop.Add("Owner");
            prop.Add("Location");

            switch (type) {
                case Equipment.EquipmentType.Laptop:
                case Equipment.EquipmentType.Chromebook:
                case Equipment.EquipmentType.Mac:
                case Equipment.EquipmentType.Desktop:
                case Equipment.EquipmentType.Tablet:

                    break;
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
                case Equipment.EquipmentType.Misc:
                    break;
                default:
                    break;
            }

            prop.Add("Notes");

            return prop;
        }




        private List<List<string>> GetEquipmentPropertyValue(Equipment.EquipmentType type, List<Equipment> data) {

            var prop = new List<List<string>>();

            for (int i = 0; i < data.Count; i++) {

                var values = new List<string>();

                values.Add(data[i].LastEdited.ToString());
                values.Add(data[i].Model);
                values.Add(data[i].Serial);

                values.Add(data[i].Owner.FullName);
                values.Add(data[i].Location);

                switch (type) {
                    case Equipment.EquipmentType.Laptop:
                    case Equipment.EquipmentType.Chromebook:
                    case Equipment.EquipmentType.Mac:
                    case Equipment.EquipmentType.Desktop:
                    case Equipment.EquipmentType.Tablet:

                        break;
                    case Equipment.EquipmentType.Projector:
                        values.Add(data[i].Resolution);
                        break;

                    case Equipment.EquipmentType.Mobile:
                        values.Add(data[i].MobileNumber);
                        break;
                    case Equipment.EquipmentType.Printer:
                        values.Add(data[i].IP);
                        break;
                    case Equipment.EquipmentType.Router:
                    case Equipment.EquipmentType.Switch:
                        values.Add(data[i].IP);
                        values.Add(data[i].Ports.ToString());
                        break;
                    case Equipment.EquipmentType.Misc:
                        break;
                    default:
                        break;
                }

                values.Add(data[i].Notes);

                prop.Add(values);
            }

            return prop;
        }


    }

    public class Property {

        public string Name { get; set; }
        public string Value { get; set; }

        public Property(string name, string val) { Name = name; Value = val; }
    }

    public class ExportFile {

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }



}
