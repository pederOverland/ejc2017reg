using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    public IActionResult Login([FromQuery] string ReturnUrl = "/")
    {
        return new ChallengeResult("Auth0", new AuthenticationProperties() { RedirectUri = ReturnUrl });
    }

    public async Task Logout()
    {
        await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
        {
            RedirectUri = Url.Action("Index", "Home")
        });
        await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}