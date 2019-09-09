using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data.Export {


    public class ExcelExporter : IExporter {


        public async Task<ExportFile> FormatData<T>(IEnumerable<T> data, string fileName) {

            var type = data.GetType().GetGenericArguments()[0];

            using (var package = new ExcelPackage()) {

                var file = new ExportFile(await CreatePackage(package, data, type));
                SetFileInfo(file, fileName);

                return file;
            }
        }


        private async Task<byte[]> CreatePackage<T>(ExcelPackage package, IEnumerable<T> data, Type type) {

            var propertyNames = new List<string>();
            var propertyInfo = GetPropertyInfo(type, propertyNames);


            switch (type.Name) {
                case "Equipment":

                    package.Workbook.Properties.Title = "Equipment";
                    var equipment = SortEquipmentByCategory((IEnumerable<Equipment>)data);

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

                    List<Owner> ownerData = ((IEnumerable<Owner>)data).ToList();

                    sheet.InsertRow(3, ownerData.Count);
                    sheet = AddPropertyHeader(sheet, propertyNames);

                    for (int i = 0; i < ownerData.Count; i++) {

                        var values = GetPropertyValue<Owner>(ownerData[i], propertyInfo);
                        sheet = AddPropertyValues(sheet, i + 3, values);
                    }

                    sheet.Cells.AutoFitColumns(0);

                    break;
            }

            package.Workbook.Properties.Company = $"Övertorneå Kommun {DateTime.Now.ToString("dd/MM/YYYY")}";


            return await Task.Run(() => package.GetAsByteArray());
        }


        private List<PropertyInfo> GetPropertyInfo(Type type, List<string> propertyNames) {

            var properties = new List<PropertyInfo>();

            for (int i = 0; i < propertyNames.Count; i++) {

                properties.Add(type.GetProperty(propertyNames[i]));
            }

            return properties;
        }


        private List<string> GetPropertyValue<T>(T instance, List<PropertyInfo> properties) {

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


        private Dictionary<Equipment.EquipmentType, List<Equipment>> SortEquipmentByCategory(IEnumerable<Equipment> data) {

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

        private List<Equipment> GetEquipment(IEnumerable<Equipment> data, Equipment.EquipmentType equipType) {

            return (from m in data
                    where m.EquipType == equipType
                    select m).ToList();
        }


        private List<string> GetEquipmentPropertyNames(Equipment.EquipmentType type, Equipment equipment) {

            var prop = new List<string> {
                "LastEdited",
                "Model",
                "Serial",

                "OwnerName",
                "Location"
            };

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


        public ExportFile SetFileInfo(ExportFile file, string fileName) {

            file.ContentType = "application/excel";
            file.FileName = fileName + ".xlsx";

            return file;
        }
    }
}
