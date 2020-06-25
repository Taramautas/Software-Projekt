using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Uebungsprojekt.Models;

public class UserManager
{

    public UserManager()
    {
    }

    public async void SignIn(HttpContext httpContext, User user)
    {
        
        // User matching_user = UserDaoImpl.GetDao().GetUserByID(user.Id); TODO: Replace line when UserDaoImpl is implemented
        User matching_user = new User()
        {
            email = "radi.achkik@gmail.com",
            Id = 2,
            name = "Radi Achkik",
            password = "asdf",
            role = Role.Employee
        };
        
        if (matching_user.email != user.email || matching_user.password != user.password)
        {
            return;
        }
        
        ClaimsIdentity identity = new ClaimsIdentity(GetUserClaims(matching_user), CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        
    }

    public async void SignOut(HttpContext httpContext)
    {
        await httpContext.SignOutAsync();
    }

    private IEnumerable<Claim> GetUserClaims(User user)
    {
        List<Claim> claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Email, user.email));
        claims.AddRange(GetUserRoleClaims(user));
        return claims;
    }

    private IEnumerable<Claim> GetUserRoleClaims(User user)
    {
        List<Claim> claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Role, user.role.ToString()));
        return claims;
    }
}