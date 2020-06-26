using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Login(string ReturnUrl)
        {
            LoginViewModel login_try = new LoginViewModel()
            {
                redirect_url = ReturnUrl,
            };
            return View(login_try);
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
                return Redirect(form.redirect_url);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error");
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
