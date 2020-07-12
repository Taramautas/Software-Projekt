using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;
using Uebungsprojekt.ViewModel.Home;

namespace Uebungsprojekt.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>Responsible for identifying and authenticating the users</summary>
        private readonly UserManager user_manger;

        private int user_id;

        private IMemoryCache cache;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager">UserManager</param>
        public HomeController(UserManager user_manager, IHttpContextAccessor http_context_accessor, IMemoryCache cache)
        {
            this.cache = cache;
            user_manger = user_manager;
            user_id = user_manager.GetUserIdByHttpContext(http_context_accessor.HttpContext);
        }

        /// <summary>
        /// Index View with overall information
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Every action requiring authentication is calling this method
        /// </summary>
        /// <param name="ReturnUrl">The URL to redirect after successfull login</param>
        /// <returns>LoginViewModel</returns>
        [HttpGet("{ReturnUrl}")]
        public IActionResult Login(string ReturnUrl)
        {
    
            
            LoginViewModel login_try = new LoginViewModel()
            {
                redirect_url = ReturnUrl,
            };
            return View(login_try);
        }
        
        /// <summary>
        /// Every action requiring authentication is calling this method
        /// </summary>
        /// <returns>LoginViewModel</returns>
        [HttpGet]
        public IActionResult Login()
        {
            if (user_id != -1)
                return RedirectToAction("Index");
            
            LoginViewModel login_try = new LoginViewModel();
            return View(login_try);
        }

        /// <summary>
        /// Try to login the user and redirect to given URL if successfull
        /// TODO: Password should not be transmitted without enryption
        /// </summary>
        /// <param name="form">Form containing user email and password</param>
        /// <returns>Requested Page</returns>
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

                if (form.redirect_url == null)
                    return RedirectToAction("Index");
                
                return Redirect(form.redirect_url);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error while logging in.");
                ModelState.AddModelError("summary", e.Message);
                return View(form);
            }
        }

        /// <summary>
        /// Logout the user
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            user_manger.SignOut(HttpContext);
            return RedirectToAction("Index");
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
