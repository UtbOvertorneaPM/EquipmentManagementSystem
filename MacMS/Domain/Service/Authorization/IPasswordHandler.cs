using EquipmentManagementSystem.Domain.Data.DbAccess;

namespace EquipmentManagementSystem.Domain.Service.Authorization {
    public interface IPasswordHandler {
        bool Validate(User user, string password);
    }
}