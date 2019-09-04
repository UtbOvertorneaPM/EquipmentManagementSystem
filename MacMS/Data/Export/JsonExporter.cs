using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data.Export {

    public class JsonExporter : IExporter {


        public async Task<byte[]> Export<T>(IEnumerable<T> data) {

            var json = await Task.Run(() => JsonConvert.SerializeObject(data, Formatting.Indented));
            return await Task.Run(() => Encoding.ASCII.GetBytes(json));
        }
    }
}
