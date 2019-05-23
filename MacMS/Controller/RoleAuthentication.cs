using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace EquipmentManagementSystem
{
    public class RoleAuthentication : AuthorizationHandler<RoleRequirement> {

        //AuthorizationHandler<T>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            if (!(context.User.Identity is WindowsIdentity windowsIdentity))
                return Task.CompletedTask;


            var windowsUser = new WindowsPrincipal(windowsIdentity);
            var identity = windowsUser.Identity as WindowsIdentity;

            var groupNames = from id in identity.Groups
                             select id.Translate(typeof(NTAccount)).Value;

            try
            {
                var hasRole = false;
                for (int i = 0; i < requirement.GroupName.Count(); i++) {

                    hasRole = windowsUser?.IsInRole(requirement.GroupName[i]) ?? false;
                    if (hasRole) { break; }
                }
                
                if (hasRole) { 

                    context.Succeed(requirement);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Task.CompletedTask;
        }



    }

    //IAuthorizationRequirement
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RoleRequirement(string[] groupName)
        { GroupName = groupName; }

        /// <summary>
        /// The Windows / AD Group Name that is allowed to call the OMS API
        /// </summary>
        public string[] GroupName { get; }
    }
}
