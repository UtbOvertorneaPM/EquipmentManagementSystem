using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using OwnerManagementSystem.Domain.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data {


    public class OwnerViewModel {

        public Owner Owner { get; set; }
        public IEnumerable<Equipment> Equipment { get; set; }


        public async Task AddEquipment(IRequestHandler service, string serial) {

            Equipment = await service.Get<Equipment>(o => o.Serial == serial).ToListAsync();
        }
    }
}
