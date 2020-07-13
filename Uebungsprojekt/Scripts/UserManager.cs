using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.DAO;

namespace Uebungsprojekt.Models
{
    public class UserManager
    {
        private UserDao user_dao;
        public UserManager(IMemoryCache cache)
        {
            user_dao = new UserDaoImpl(cache);
        }

        public async void SignIn(HttpContext httpContext, User user)
        {
            User matching_user = user_dao.GetByEmail(user.email);

            if (matching_user != null && matching_user.password == user.password)
            {
                ClaimsIdentity identity = new ClaimsIdentity(GetUserClaims(matching_user), CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
        }

        public async void SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
        }

        public int GetUserIdByHttpContext(HttpContext httpContext)
        {
            var user_id = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            
            return user_id != null ? Int16.Parse(user_id.Value) : -1;
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.email));
            claims.AddRange(GetUserRoleClaims(user));
            return claims;
        }

        private IEnumerable<Claim> GetUserRoleClaims(User user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, user.role.ToString()));
            return claims;
        }
    }
}