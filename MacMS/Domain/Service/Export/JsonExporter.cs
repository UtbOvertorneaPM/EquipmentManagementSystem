using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data.Export {

    public class JsonExporter : IExporter {


        public async Task<ExportFile> FormatData<T>(IEnumerable<T> data, string fileName) {

            var json = await Task.Run(() => JsonConvert.SerializeObject(data, Formatting.Indented));
            var file = new ExportFile(await Task.Run(() => Encoding.ASCII.GetBytes(json)));
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
