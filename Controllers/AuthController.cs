using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHR_Payroll.Services.IServices;
using System.Security.Claims;

namespace SmartHR_Payroll.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            var authenticateResult = HttpContext.AuthenticateAsync("ApplicationCookie").Result;
            if (authenticateResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        public IActionResult LoginWithGoogle(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("ExternalCookie");

            if (!authenticateResult.Succeeded)
                return RedirectToAction("Login", new { error = "Lỗi khi kết nối với Google. Vui lòng thử lại." });

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var avatarUrl = authenticateResult.Principal.FindFirstValue("urn:google:picture");

            var principal = await _authService.ValidateGoogleUserAsync(email, avatarUrl);

            if (principal == null)
            {
                await HttpContext.SignOutAsync("ExternalCookie");
                return RedirectToAction("AccessDenied");
            }

            await HttpContext.SignInAsync("ApplicationCookie", principal);

            await HttpContext.SignOutAsync("ExternalCookie");

            return LocalRedirect(returnUrl);
        }


        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("ApplicationCookie");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(); 
        }

        [HttpGet("/Account/AccessDenied")]
        public IActionResult AccountAccessDenied()
        {
            return RedirectToAction(nameof(AccessDenied));
        }
    }
}
