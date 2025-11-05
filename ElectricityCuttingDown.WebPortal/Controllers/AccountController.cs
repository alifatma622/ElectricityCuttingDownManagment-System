using ElectricityCuttingDown.WebPortal.Models.ViewModels;
using ElectricityCuttingDown.WebPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElectricityCuttingDown.WebPortal.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validation
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View(model);
            }

            // Authenticate with database
            var user = await _authService.AuthenticateAsync(model.Username, model.Password);

            if (user != null)
            {
                // Set session
                HttpContext.Session.SetString("UserId", user.User_Key.ToString());
                HttpContext.Session.SetString("Username", user.Name);
                HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                _logger.LogInformation($"User logged in: {user.Name}");

                // Redirect to dashboard
                return RedirectToAction("Index", "Home");
            }

            // Failed login
            ModelState.AddModelError("", "Invalid username or password.");
            _logger.LogWarning($"Failed login attempt for: {model.Username}");
            return View(model);
        }

        public IActionResult Logout()
        {
            var username = HttpContext.Session.GetString("Username");
            HttpContext.Session.Clear();

            _logger.LogInformation($"User logged out: {username}");

            return RedirectToAction("Login");
        }
    }
}
