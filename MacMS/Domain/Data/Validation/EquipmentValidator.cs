using EquipmentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Domain.Service;
using Microsoft.EntityFrameworkCore;
using EquipmentManagementSystem.Domain.Business;

namespace EquipmentManagementSystem.Domain.Data.Validation {


    public class EquipmentValidator : IValidator {

        private IRequestHandler _service;

        public EquipmentValidator(IRequestHandler service) => _service = service;

        public async Task<bool> Validate<T>(T entity) where T : class {

            var isValid = true;
            
            if (entity is Equipment equipment) {

                if (string.IsNullOrEmpty(equipment.Serial)) {
                    isValid = false;
                }

                var eqp = await _service.Get<Equipment>(e => e.Serial == equipment.Serial).FirstAsync();
                if (eqp != null && eqp.EquipType == equipment.EquipType) {

                    isValid = false;
                }
            }
            else {

                isValid = false;
            }

            return isValid;
        }
    }
}
