using EquipmentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EquipmentManagementSystem.Data;

namespace EquipmentManagementSystem {

    public static class Util {


        public static string GetFullName(Owner owner) {


            if (owner is null || owner.FirstName is null || owner.LastEdited == null) {

                return "";
            }

            return owner.FirstName + " " + owner.LastName;
        }
    }

}
