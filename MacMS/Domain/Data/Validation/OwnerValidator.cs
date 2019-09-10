using EquipmentManagementSystem.Domain.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquipmentManagementSystem.Domain.Service;
using Microsoft.EntityFrameworkCore;
using EquipmentManagementSystem.Domain.Business;

namespace EquipmentManagementSystem.Domain.Data.Validation {


    public class OwnerValidator : IValidator {

        private IRequestHandler _service;

        public OwnerValidator(IRequestHandler service) => _service = service;

        public async Task<bool> Validate<T>(T entity) where T : class {

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

                if (await _service.Get<Owner>(o => o.Mail == owner.Mail).CountAsync() > 0) {

                    isValid = false;
                }

                if (await _service.Get<Owner>(o => o.FullName == owner.FullName).CountAsync() > 0) {

                    isValid = false;
                }
            }


            return isValid;
        }
    }
}
