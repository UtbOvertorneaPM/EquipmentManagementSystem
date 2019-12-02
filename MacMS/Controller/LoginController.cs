using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Data.Models;
using EquipmentManagementSystem.Domain.Service.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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



        public async Task<IActionResult> PostLogin(User user) {

            _context.Database.EnsureCreated();

            var dbUser = _context.Users.Where(u => u.name == user.name).FirstOrDefault();

            if (dbUser != null && _passwordHandler.Validate(dbUser, user.password)) {

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.name));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.id.ToString()));

                var principal = new ClaimsPrincipal(identity);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(Login));
        }


    }
}