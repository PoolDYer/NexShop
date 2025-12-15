using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexShop.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El correo es requerido")]
            [EmailAddress(ErrorMessage = "El correo debe ser valido")]
            [Display(Name = "Correo Electronico")]
            public string Email { get; set; } = default!;

            [Required(ErrorMessage = "La contrasena es requerida")]
            [DataType(DataType.Password)]
            [Display(Name = "Contrasena")]
            public string Password { get; set; } = default!;

            [Display(Name = "Recuerdame")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Si ya está autenticado, redirigir a home
            if (User.Identity?.IsAuthenticated == true)
            {
                Response.Redirect("/", false);
                return;
            }

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Intentar encontrar usuario por email
                var user = await _userManager.FindByEmailAsync(Input.Email);
                
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos");
                    _logger.LogWarning("Intento de login fallido - Usuario no encontrado: {Email}", Input.Email);
                    return Page();
                }

                // Intentar sign in
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName, 
                    Input.Password, 
                    Input.RememberMe, 
                    lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Email} inicio sesion correctamente.", Input.Email);
                    return RedirectToPage("/Index", new { area = "" });
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Cuenta de usuario {Email} bloqueada.", Input.Email);
                    return RedirectToPage("./Lockout");
                }
                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                
                ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos");
                _logger.LogWarning("Intento fallido de inicio de sesion para {Email}.", Input.Email);
            }

            return Page();
        }
    }
}
