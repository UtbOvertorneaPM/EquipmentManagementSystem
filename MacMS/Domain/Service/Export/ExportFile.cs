
namespace EquipmentManagementSystem.Domain.Service.Export {

    public class ExportFile {

        public string FileName { get; set; }

        public string ContentType { get; set; }
        public byte[] Data { get; set; }


        public ExportFile(byte[] data) {

            Data = data;
        }



    }
}
