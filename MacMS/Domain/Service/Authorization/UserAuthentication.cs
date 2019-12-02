using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Data.Models;
using Microsoft.AspNetCore.Authorization;

using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service.Authorization {


    public class UserAuthenticationHandler : AuthorizationHandler<UserRequirement> {


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement) {


            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }


    public class UserRequirement : IAuthorizationRequirement {

        public User[] Users { get;}

        public UserRequirement(User[] users) {

            Users = users;
        }
    }
}
