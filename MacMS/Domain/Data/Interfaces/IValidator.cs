using System.Threading.Tasks;

namespace EquipmentManagementSystem.newData.Validation {


    public interface IValidator {


        bool Validate<T>(T entity) where T : class;
    }
}