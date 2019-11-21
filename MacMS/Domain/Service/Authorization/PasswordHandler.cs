using EquipmentManagementSystem.Domain.Data.DbAccess;
using System;

using System.Linq;
using System.Security.Cryptography;


namespace EquipmentManagementSystem.Domain.Service.Authorization {

    public class PasswordHandler : IPasswordHandler {

        public bool Validate(User user, string password) {

            var passwordInfo = user.password.Split(".");

            using (var algorithm = new Rfc2898DeriveBytes(password, Convert.FromBase64String(passwordInfo[0]), 1000, HashAlgorithmName.SHA512)) {

                var keyToCheck = algorithm.GetBytes(512);

                return keyToCheck.SequenceEqual(Convert.FromBase64String(passwordInfo[1]));
            }
        }
    }
}
