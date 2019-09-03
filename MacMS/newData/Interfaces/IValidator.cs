using System.Threading.Tasks;

namespace EquipmentManagementSystem.newData.Validation {


    public interface IValidator<T> where T : class {


        Task<bool> Validate(T entity);
    }
}