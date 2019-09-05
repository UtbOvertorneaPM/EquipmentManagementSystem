using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data.Export {
    public interface IExporter {

        Task<ExportFile> FormatData<T>(IEnumerable<T> data, string fileName);
        ExportFile SetFileInfo(ExportFile file, string fileName);
    }
}
