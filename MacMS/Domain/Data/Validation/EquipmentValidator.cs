using EquipmentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.newData.Validation {

    public class EquipmentValidator : IValidator {

        public bool Validate<T>(T entity) where T : class {

            var isValid = true;

            if (entity is Equipment equipment) {

                if (string.IsNullOrEmpty(equipment.Serial)) {
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}
