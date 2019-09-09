using EquipmentManagementSystem.Domain.Business;
using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Domain.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data {

    public class EquipmentViewModel {

        public Equipment Equipment { get; set; }
        public Owner Owner { get; set; }


        public EquipmentViewModel() { }

        public async Task AddOwner(EquipmentRequestHandler service, string name) {

            Owner = await service.Get<Owner>(o => o.FullName == name).FirstOrDefaultAsync();
        }
    }
}
