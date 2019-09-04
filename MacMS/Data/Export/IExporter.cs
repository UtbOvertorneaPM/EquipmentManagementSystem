using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Data.Export {
    public interface IExporter {

        Task<byte[]> Export<T>(IEnumerable<T> data);
    }
}
