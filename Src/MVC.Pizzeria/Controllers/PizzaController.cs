using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Pizzeria.Models;
using MVC.Pizzeria.Services;
using System.Net;
using System.Text;

namespace MVC.Pizzeria.Controllers;

public class PizzaController : Controller
{
    private const long TamanoMaximoImagen = 5 * 1024 * 1024;
    private readonly ApiService _apiService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PizzaController> _logger;

    public PizzaController(
        ApiService apiService,
        IWebHostEnvironment environment,
        ILogger<PizzaController> logger)
    {
        _apiService = apiService;
        _environment = environment;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var pizzas = await _apiService.GetAsync<List<Pizza>>("api/pizzas/");
        return View(pizzas ?? new List<Pizza>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Crear(
        Pizza pizza,
        string? ingredientes,
        IFormFile? imagen,
        string? imagenUrl)
    {
        if (!ValidarPizza(pizza))
            return VolverAlMenuConError("Completá el nombre, la descripción y un precio válido.");

        if (imagen is not null && !string.IsNullOrWhiteSpace(imagenUrl))
            return VolverAlMenuConError("Elegí subir un archivo o pegar una URL, no ambas opciones.");

        if (!TryNormalizarImagenUrl(imagenUrl, out var urlNormalizada))
            return VolverAlMenuConError("La URL de imagen debe ser HTTP o HTTPS y no superar 255 caracteres.");

        if (imagen is null && urlNormalizada is null)
            return VolverAlMenuConError("Seleccioná una imagen o pegá la URL de una imagen.");

        pizza.Ingredientes = SepararIngredientes(ingredientes);
        string? imagenLocalUrl = null;
        var idsExistentes = new HashSet<int>();
        try
        {
            var pizzasExistentes = await _apiService.GetAsync<List<Pizza>>("api/pizzas/");
            idsExistentes = pizzasExistentes?.Select(actual => actual.IdPizza).ToHashSet()
                ?? new HashSet<int>();

            if (imagen is not null)
            {
                imagenLocalUrl = await GuardarImagenAsync(imagen);
                pizza.ImagenUrl = imagenLocalUrl;
            }
            else
            {
                pizza.ImagenUrl = urlNormalizada;
            }

            var creada = await _apiService.PostAsync<Pizza, Pizza>("api/pizzas/", pizza);
            if (creada is null)
            {
                var pizzasActuales = await _apiService.GetAsync<List<Pizza>>("api/pizzas/");
                creada = pizzasActuales?.FirstOrDefault(actual =>
                    !idsExistentes.Contains(actual.IdPizza)
                    && actual.Nombre == pizza.Nombre
                    && actual.Precio == pizza.Precio
                    && actual.ImagenUrl == pizza.ImagenUrl);

                if (creada is null)
                {
                    _logger.LogWarning("La API no confirmó la creación de la pizza con imagen {ImagePath}", pizza.ImagenUrl);
                    TempData["ErrorAdmin"] = "La API respondió sin confirmar la pizza. Revisá el menú y la conexión con la base antes de reintentar.";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["MensajeAdmin"] = "Pizza agregada al menú.";
        }
        catch (InvalidDataException ex)
        {
            TempData["ErrorAdmin"] = ex.Message;
        }
        catch (HttpRequestException ex)
        {
            if (imagenLocalUrl is not null)
                _logger.LogWarning(ex, "No se pudo confirmar la creación de la pizza; se conserva la imagen {ImagePath}", imagenLocalUrl);

            TempData["ErrorAdmin"] = ex.StatusCode == HttpStatusCode.InternalServerError
                ? "La API no pudo guardar la pizza. Verificá que ejecutaste Scripts/MigracionImagenPizza.sql en la base de datos."
                : ex.StatusCode is null
                    ? "No se pudo conectar con la API. Iniciá Pizzeria.API en localhost:5260 y revisá la base de datos."
                    : "La API rechazó la creación de la pizza. Revisá los datos e intentá nuevamente.";
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "No se pudo guardar la imagen de la pizza");
            TempData["ErrorAdmin"] = "No se pudo guardar la imagen en el servidor.";
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "No hay permisos para guardar la imagen de la pizza");
            TempData["ErrorAdmin"] = "El servidor no tiene permisos para guardar la imagen.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Editar(
        Pizza pizza,
        string? ingredientes,
        IFormFile? imagen,
        string? imagenUrl)
    {
        if (!ValidarPizza(pizza))
            return VolverAlMenuConError("Completá el nombre, la descripción y un precio válido.");

        if (imagen is not null && !string.IsNullOrWhiteSpace(imagenUrl))
            return VolverAlMenuConError("Elegí subir un archivo o pegar una URL, no ambas opciones.");

        if (!TryNormalizarImagenUrl(imagenUrl, out var urlNormalizada))
            return VolverAlMenuConError("La URL de imagen debe ser HTTP o HTTPS y no superar 255 caracteres.");

        pizza.Ingredientes = SepararIngredientes(ingredientes);
        string? nuevaImagenUrl = null;
        try
        {
            var existente = await _apiService.GetAsync<Pizza>($"api/pizzas/{pizza.IdPizza}");
            if (existente is null)
            {
                TempData["ErrorAdmin"] = "No se encontró la pizza que intentás editar.";
                return RedirectToAction(nameof(Index));
            }

            pizza.ImagenUrl = existente.ImagenUrl;
            if (imagen is not null)
            {
                nuevaImagenUrl = await GuardarImagenAsync(imagen);
                pizza.ImagenUrl = nuevaImagenUrl;
            }
            else if (urlNormalizada is not null)
            {
                pizza.ImagenUrl = urlNormalizada;
            }

            await _apiService.PutAsync($"api/pizzas/{pizza.IdPizza}", pizza);
            if (pizza.ImagenUrl != existente.ImagenUrl)
                EliminarImagenLocal(existente.ImagenUrl);

            TempData["MensajeAdmin"] = "Pizza actualizada.";
        }
        catch (InvalidDataException ex)
        {
            TempData["ErrorAdmin"] = ex.Message;
        }
        catch (HttpRequestException ex)
        {
            if (nuevaImagenUrl is not null)
                _logger.LogWarning(ex, "No se pudo confirmar el cambio de imagen de la pizza {PizzaId}", pizza.IdPizza);

            TempData["ErrorAdmin"] = ex.StatusCode == HttpStatusCode.InternalServerError
                ? "La API no pudo actualizar la pizza. Verificá que ejecutaste Scripts/MigracionImagenPizza.sql en la base de datos."
                : ex.StatusCode is null
                    ? "No se pudo conectar con la API. Iniciá Pizzeria.API en localhost:5260 y revisá la base de datos."
                    : "La API rechazó la actualización. Revisá los datos e intentá nuevamente.";
        }
        catch (IOException ex)
        {
            if (nuevaImagenUrl is not null)
                EliminarImagenLocal(nuevaImagenUrl);

            _logger.LogError(ex, "No se pudo guardar la nueva imagen para la pizza {PizzaId}", pizza.IdPizza);
            TempData["ErrorAdmin"] = "No se pudo guardar la imagen en el servidor.";
        }
        catch (UnauthorizedAccessException ex)
        {
            if (nuevaImagenUrl is not null)
                EliminarImagenLocal(nuevaImagenUrl);

            _logger.LogError(ex, "No hay permisos para guardar la nueva imagen de la pizza {PizzaId}", pizza.IdPizza);
            TempData["ErrorAdmin"] = "El servidor no tiene permisos para guardar la imagen.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int idPizza)
    {
        try
        {
            var pizza = await _apiService.GetAsync<Pizza>($"api/pizzas/{idPizza}");
            await _apiService.DeleteAsync($"api/pizzas/{idPizza}");
            EliminarImagenLocal(pizza?.ImagenUrl);
            TempData["MensajeAdmin"] = "Pizza eliminada del menú.";
        }
        catch (HttpRequestException)
        {
            TempData["ErrorAdmin"] = "No se pudo eliminar la pizza. Puede tener pedidos asociados.";
        }

        return RedirectToAction(nameof(Index));
    }

    private static bool ValidarPizza(Pizza pizza) =>
        !string.IsNullOrWhiteSpace(pizza.Nombre)
        && !string.IsNullOrWhiteSpace(pizza.Descripcion)
        && pizza.Precio > 0;

    private static List<string> SepararIngredientes(string? ingredientes) =>
        string.IsNullOrWhiteSpace(ingredientes)
            ? new List<string>()
            : ingredientes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    private static bool TryNormalizarImagenUrl(string? valor, out string? imagenUrl)
    {
        imagenUrl = null;
        if (string.IsNullOrWhiteSpace(valor))
            return true;

        if (!Uri.TryCreate(valor.Trim(), UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || uri.AbsoluteUri.Length > 255)
            return false;

        imagenUrl = uri.AbsoluteUri;
        return true;
    }

    private async Task<string> GuardarImagenAsync(IFormFile imagen)
    {
        if (imagen.Length <= 0 || imagen.Length > TamanoMaximoImagen)
            throw new InvalidDataException("La imagen debe pesar como máximo 5 MB.");

        var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();
        if (extension is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            throw new InvalidDataException("Usá una imagen JPG, PNG o WebP.");

        var encabezado = new byte[12];
        await using (var stream = imagen.OpenReadStream())
        {
            var leidos = 0;
            while (leidos < encabezado.Length)
            {
                var cantidadLeida = await stream.ReadAsync(
                    encabezado.AsMemory(leidos),
                    HttpContext.RequestAborted);
                if (cantidadLeida == 0)
                    break;

                leidos += cantidadLeida;
            }

            if (!EsImagenValida(extension, encabezado, leidos))
                throw new InvalidDataException("El archivo seleccionado no es una imagen válida.");
        }

        var carpetaWeb = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var carpetaImagenes = Path.Combine(carpetaWeb, "images", "pizzas");
        Directory.CreateDirectory(carpetaImagenes);

        var nombreArchivo = $"{Guid.NewGuid():N}{extension}";
        var ruta = Path.Combine(carpetaImagenes, nombreArchivo);
        try
        {
            await using var archivo = new FileStream(
                ruta, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await imagen.CopyToAsync(archivo, HttpContext.RequestAborted);
        }
        catch (IOException)
        {
            if (System.IO.File.Exists(ruta))
                EliminarImagenLocal($"/images/pizzas/{nombreArchivo}");
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            if (System.IO.File.Exists(ruta))
                EliminarImagenLocal($"/images/pizzas/{nombreArchivo}");
            throw;
        }
        catch (OperationCanceledException)
        {
            if (System.IO.File.Exists(ruta))
                EliminarImagenLocal($"/images/pizzas/{nombreArchivo}");
            throw;
        }

        return $"/images/pizzas/{nombreArchivo}";
    }

    private static bool EsImagenValida(string extension, byte[] encabezado, int leidos) =>
        extension switch
        {
            ".jpg" or ".jpeg" =>
                leidos >= 3 && encabezado[0] == 0xFF && encabezado[1] == 0xD8 && encabezado[2] == 0xFF,
            ".png" =>
                leidos >= 8 && encabezado.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            ".webp" =>
                leidos >= 12
                && Encoding.ASCII.GetString(encabezado, 0, 4) == "RIFF"
                && Encoding.ASCII.GetString(encabezado, 8, 4) == "WEBP",
            _ => false
        };

    private void EliminarImagenLocal(string? imagenUrl)
    {
        const string prefijo = "/images/pizzas/";
        if (string.IsNullOrWhiteSpace(imagenUrl)
            || !imagenUrl.StartsWith(prefijo, StringComparison.Ordinal))
            return;

        var nombreArchivo = Path.GetFileName(imagenUrl);
        var carpetaWeb = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var carpetaImagenes = Path.GetFullPath(Path.Combine(carpetaWeb, "images", "pizzas"));
        var ruta = Path.GetFullPath(Path.Combine(carpetaImagenes, nombreArchivo));
        if (!ruta.StartsWith(carpetaImagenes + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            if (System.IO.File.Exists(ruta))
                System.IO.File.Delete(ruta);
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "No se pudo eliminar la imagen local {ImagePath}", ruta);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "No se pudo eliminar la imagen local {ImagePath}", ruta);
        }
    }

    private IActionResult VolverAlMenuConError(string mensaje)
    {
        TempData["ErrorAdmin"] = mensaje;
        return RedirectToAction(nameof(Index));
    }
}
