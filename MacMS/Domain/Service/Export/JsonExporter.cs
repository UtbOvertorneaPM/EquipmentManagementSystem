using EquipmentManagementSystem.Domain.Service.Export;
using Newtonsoft.Json;

using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data.Export {

    public class JsonExporter : IExporter {


        public async Task<ExportFile> FormatData<T>(IEnumerable<T> data, string fileName) {

            var json = await Task.Run(() => JsonConvert.SerializeObject(data, Formatting.Indented));
            var file = new ExportFile(await Task.Run(() => Encoding.UTF8.GetBytes(json)));
            SetFileInfo(file, fileName);

            return file;
        }

        public ExportFile SetFileInfo(ExportFile file, string fileName) {

            file.ContentType = "text/json";
            file.FileName = fileName + ".json";

            return file;
        }
    }
}
