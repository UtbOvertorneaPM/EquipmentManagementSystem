using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data.Models;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data {

    public class EquipmentViewModel {

        public Equipment Equipment { get; set; }
        public Owner Owner { get; set; }


        public EquipmentViewModel() { }

        public async Task AddOwner(IRequestHandler service, string name) {

            Owner = await service.Get<Owner>(o => o.FullName == name).FirstOrDefaultAsync();
        }
    }
}
