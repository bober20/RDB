using System.Security.Claims;
using CleaningServiceLab6.Models;
using CleaningServiceLab6.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CleaningServiceLab6.Controllers;

public class AccountController(DbService dbService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> LoginHandler(LoginViewModel loginViewModel)
    {
        var user = await dbService.AuthenticateUser(loginViewModel);
        
        if (user != null)
        {
            await Authenticate(loginViewModel.Email, user.UserId, user.IsStaff, user.IsAdmin);
            
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Login", "Account");
    }
    
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }
    
    public async Task<IActionResult> LogoutHandler()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
    
    public IActionResult Registration()
    {
        return View(new RegistrationViewModel());
    }
    
    public async Task<IActionResult> RegistrationHandler(RegistrationViewModel registrationViewModel)
    {
        if (await dbService.RegisterUser(registrationViewModel))
        {
            return RedirectToAction("Login", "Account");
        }

        return RedirectToAction("Registration", "Account");
    }
    
    private async Task Authenticate(string userName, int userId, bool isStaff, bool isAdmin)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("is_staff", isStaff.ToString()),
            new Claim("is_admin", isAdmin.ToString())
        };
        
        ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
       
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }
}
