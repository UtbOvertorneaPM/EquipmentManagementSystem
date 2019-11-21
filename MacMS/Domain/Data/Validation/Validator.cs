using System.Threading.Tasks;
using EquipmentManagementSystem.Domain.Data.Models;

using EquipmentManagementSystem.Domain.Business;

namespace EquipmentManagementSystem.Domain.Data.Validation {


    public class Validator : IValidator {

        private IRequestHandler _service;

        public Validator(IRequestHandler service) => _service = service;

        public async Task<bool> Validate<T>(T entity) where T : class {

            var isValid = true;

            if (entity is Equipment equipment) {

                if (string.IsNullOrEmpty(equipment.Serial)) {
                    isValid = false;
                }
            }
            else if (entity is Owner owner) {

                if (string.IsNullOrEmpty(owner.FirstName)) {

                    isValid = false;
                }

                if (string.IsNullOrEmpty(owner.LastName)) {

                    isValid = false;
                }
                /*
                if (await _service.Get<Owner>(o => o.FullName == owner.FullName).CountAsync() > 0) {

                    isValid = false;
                }
                */
            }

            return isValid;
        }
    }
}
