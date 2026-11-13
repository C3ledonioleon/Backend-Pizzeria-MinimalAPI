
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using MVC.Pizzeria.Models;
using MVC.Pizzeria.Services;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Pizzeria.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApiService _apiService;

        public UsuarioController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // MOSTRAR LOGIN
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // INICIAR SESIÓN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                var respuesta = await _apiService.PostAsync<
                    LoginViewModel,
                    UsuarioRespuestaMVC
                >("api/usuarios/login", modelo);

                if (respuesta == null ||
                    string.IsNullOrEmpty(respuesta.Token))
                {
                    ModelState.AddModelError(
                        "",
                        "No se pudo iniciar sesión."
                    );

                    return View(modelo);
                }

                // DATOS DEL USUARIO
             var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        respuesta.IdUsuario.ToString()
                    ),

                    new Claim(
                        ClaimTypes.Name,
                        respuesta.Username
                    ),

                    new Claim(
                        ClaimTypes.Role,
                        respuesta.Rol == 1 ? "Administrador" : "Cliente"
                    )
};

                var identidad = new ClaimsIdentity(
                    claims,
                    "Cookies"
                );

                var principal = new ClaimsPrincipal(identidad);

                // GUARDAR JWT EN LA COOKIE
                var propiedades = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                };

                propiedades.StoreTokens(new[]
                {
                    new AuthenticationToken
                    {
                        Name = "access_token",
                        Value = respuesta.Token
                    }
                });

                await HttpContext.SignInAsync(
                    "Cookies",
                    principal,
                    propiedades
                );

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "",
                    "Usuario o contraseña incorrectos, o no se pudo conectar con la API."
                );

                return View(modelo);
            }
        }

        // MOSTRAR REGISTRO
        [HttpGet]
        public IActionResult Registro(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // REGISTRAR USUARIO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                var respuesta = await _apiService.PostAsync<
                    RegistroViewModel,
                    UsuarioRespuestaMVC
                >("api/usuarios/registro", modelo);

                if (respuesta == null)
                {
                    ModelState.AddModelError(
                        "",
                        "No se pudo registrar el usuario."
                    );

                    return View(modelo);
                }

                TempData["Mensaje"] = "¡Cuenta creada correctamente! Ahora podés iniciar sesión.";

                return RedirectToAction("Login", new { returnUrl });
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "",
                    "No se pudo completar el registro. Verificá tus datos o intentá nuevamente."
                );

                return View(modelo);
            }
        }

        // CERRAR SESIÓN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");

            return RedirectToAction("Login");
        }
    }
}
