using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Data.Validation {


    public interface IValidator {


        bool Validate<T>(T entity) where T : class;
    }
}