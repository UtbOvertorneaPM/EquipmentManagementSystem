using EquipmentManagementSystem.Domain.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data.Validation {


    public class OwnerValidator : IValidator {

        public bool Validate<T>(T entity) where T : class {

            var isValid = true;

            if (entity is Owner owner) {

                if (string.IsNullOrEmpty(owner.FirstName)) {

                    isValid = false;
                }

                if (string.IsNullOrEmpty(owner.LastName)) {

                    isValid = false;
                }

                if (string.IsNullOrEmpty(owner.Mail) && string.IsNullOrEmpty(owner.TelNr)) {

                    isValid = false;
                }
            }


            return isValid;
        }
    }
}
