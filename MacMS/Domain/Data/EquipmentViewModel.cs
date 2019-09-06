using EquipmentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data {

    public class EquipmentViewModel {

        public Equipment Equipment { get; set; }
        public IEnumerable<Owner> Owners { get; set; }
        public int SelectedOwner { get; set; }

    }
}
