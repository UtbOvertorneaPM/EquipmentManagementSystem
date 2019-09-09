using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data {

    public class ExportFile {

        public string FileName { get; set; }

        public string ContentType { get; set; }
        public byte[] Data { get; set; }


        public ExportFile(byte[] data) {

            Data = data;
        }



    }
}
