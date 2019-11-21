using System.Linq;

using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Service.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagementSystem.Controllers
{
    public class LoginController : Microsoft.AspNetCore.Mvc.Controller {


        ManagementContext _context;
        IPasswordHandler _passwordHandler;


        public LoginController(ManagementContext context) {

            _context = context;
            _passwordHandler = new PasswordHandler();
        }

        public IActionResult Login() {


            return View(new User());
        }



        public IActionResult PostLogin(User user) {

            var dbUser = _context.Users.Where(u => u.name == user.name).FirstOrDefault();

            if (dbUser != null && _passwordHandler.Validate(dbUser, user.password)) {

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(Login));
        }


    }
}