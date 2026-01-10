using DafHukuk.Core.Entities;
using DafHukuk.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DafHukuk.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("🔵 Login sayfası açıldı");

            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("⚠️ Kullanıcı zaten giriş yapmış, admin'e yönlendiriliyor");
                return Redirect("/admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation($"🔵 Login POST isteği alındı: {model.Email}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ Model state geçersiz");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning($"❌ Kullanıcı bulunamadı: {model.Email}");
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"✅ Kullanıcı başarıyla giriş yaptı: {model.Email}");
                return Redirect("/admin");
            }

            _logger.LogWarning($"❌ Hatalı şifre girişi: {model.Email}");
            ModelState.AddModelError("", "Hatalı şifre.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("🔴 Logout GET isteği alındı");

            var wasAuthenticated = User.Identity?.IsAuthenticated ?? false;
            _logger.LogInformation($"🔴 Kullanıcı authenticated mı? {wasAuthenticated}");

            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("✅ SignOutAsync başarılı");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ SignOutAsync hatası: {ex.Message}");
            }

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            HttpContext.Response.Cookies.Delete(".AspNetCore.Antiforgery.xxYljYuzWuA");

            _logger.LogInformation("🟢 Cookie'ler temizlendi, Login'e yönlendiriliyor");

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutPost()
        {
            _logger.LogInformation("🔴 Logout POST isteği alındı");

            await _signInManager.SignOutAsync();

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            HttpContext.Response.Cookies.Delete(".AspNetCore.Antiforgery.xxYljYuzWuA");

            return RedirectToAction("Login");
        }
    }
}