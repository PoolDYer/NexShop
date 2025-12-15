using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexShop.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<Usuario> userManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre completo es requerido")]
            [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
            [Display(Name = "Nombre Completo")]
            public string NombreCompleto { get; set; } = default!;

            [Required(ErrorMessage = "El correo es requerido")]
            [EmailAddress(ErrorMessage = "El correo debe ser valido")]
            [Display(Name = "Correo Electronico")]
            public string Email { get; set; } = default!;

            [Required(ErrorMessage = "La contrasena es requerida")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "La contrasena debe tener al menos 8 caracteres")]
            [DataType(DataType.Password)]
            [Display(Name = "Contrasena")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                ErrorMessage = "La contrasena debe contener mayusculas, minusculas, numeros y caracteres especiales (@$!%*?&)")]
            public string Password { get; set; } = default!;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contrasena")]
            [Compare("Password", ErrorMessage = "Las contrasenas no coinciden")]
            public string ConfirmPassword { get; set; } = default!;

            [Display(Name = "Eres Vendedor?")]
            public bool EsVendedor { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (User.Identity?.IsAuthenticated == true)
            {
                Response.Redirect("/", false);
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Verificar si el email ya existe
                var usuarioExistente = await _userManager.FindByEmailAsync(Input.Email);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Input.Email", "El correo ya esta registrado");
                    return Page();
                }

                var user = new Usuario
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    NombreCompleto = Input.NombreCompleto,
                    TipoUsuario = Input.EsVendedor ? "Vendedor" : "Comprador",
                    EstaActivo = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Email} registrado exitosamente.", Input.Email);

                    // Asignar rol sin iniciar sesion
                    string rol = Input.EsVendedor ? "Vendedor" : "Comprador";
                    await _userManager.AddToRoleAsync(user, rol);

                    // NO INICIAR SESION AUTOMATICAMENTE - El usuario debe hacer login
                    TempData["SuccessMessage"] = "Registro exitoso. Por favor, inicia sesion con tus credenciales.";
                    return RedirectToPage("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
