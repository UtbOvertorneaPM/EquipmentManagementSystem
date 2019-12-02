using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Data.Models;

namespace EquipmentManagementSystem.Domain.Service.Authorization {
    public interface IPasswordHandler {
        bool Validate(User user, string password);
    }
}