using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Data.Export;
using EquipmentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service.Export {

    public class ExportService<T> where T : class {

        IExporter exporter;

        public async Task<ExportFile> Export(IEnumerable<T> data, ExportType exportType) {

            SetExporter(exportType);
            return await exporter.FormatData(data, GetFileName(data.GetType().Name));
        }


        private string GetFileName(string type) {

            var fileName = "";

            switch (type) {

                case "Equipment":

                    fileName = $"EquipmentExport-{DateTime.Now.ToString("dd/MM/yyyy")}";
                    break;

                case "Owner":

                    fileName = $"OwnerExport-{DateTime.Now.ToString("dd/MM/yyyy")}";
                    break;

                default:

                    return null;
            }

            return fileName;
        }


        private void SetExporter(ExportType exportType) {

            IExporter exporter;

            switch (exportType) {

                case ExportType.Json:

                    exporter = new JsonExporter();
                    break;

                case ExportType.Excel:

                    exporter = new ExcelExporter();
                    break;

                default:
                    throw new ArgumentNullException(nameof(exportType),
                        "You must specify an ExportType");
            }
        }

    }

    public enum ExportType {
        Json,
        Excel
    }
}
