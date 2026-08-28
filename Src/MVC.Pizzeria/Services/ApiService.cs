using System.Net.Http.Json;

namespace MVC.Pizzeria.Services
{
public class ApiService
{
private readonly HttpClient _httpClient;


    public ApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PizzeriaAPI");
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        return await _httpClient.GetFromJsonAsync<T>(endpoint);
    }
}


}
