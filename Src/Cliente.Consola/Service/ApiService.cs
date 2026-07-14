using System.Net.Http.Json;

namespace Cliente.Consola.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://localhost:5000/");
    }

    public async Task CrearPedidoAsync()
    {
        Console.WriteLine("Todavía no implementado.");
    }

    public async Task VerPedidosAsync()
    {
        Console.WriteLine("Todavía no implementado.");
    }

    public async Task VerClientesAsync()
    {
        Console.WriteLine("Todavía no implementado.");
    }
}
