using EquipmentManagementSystem.Domain.Service;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data.Validation {


    public interface IValidator {

        Task<bool> Validate<T>(T entity) where T : class;
    }
}