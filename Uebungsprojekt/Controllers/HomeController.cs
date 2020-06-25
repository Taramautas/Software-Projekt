using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;

namespace Uebungsprojekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager user_manger;

        public HomeController(UserManager userManager)
        {
            user_manger = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }
            try
            {
                var user = new User()
                {
                    email = form.email,
                    password = form.password,
                };
                
                user_manger.SignIn(HttpContext, user);
                return RedirectToPage("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("summary", e.Message);
                return View(form);
            }
        }

        public IActionResult Logout()
        {
            user_manger.SignOut(HttpContext);
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password, Role role)
        {
            /* // Try to create new user
             * bool success = UserDaoImpl.GetDao().CreateUser(username, email, password, role);
             * if (success)
             * {
             *    Http
             * }
             */
            return View();
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
