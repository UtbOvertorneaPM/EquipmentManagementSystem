using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Models {

    public class EquipmentForm  {

        public Equipment Equipment { get; set; }
        public bool IdCheck { get; set; }

        public EquipmentForm() { }

        public EquipmentForm(Equipment equipment) {

            Equipment = equipment;
        }
    }
}
