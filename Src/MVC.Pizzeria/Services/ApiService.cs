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

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
    {
        using var response = await _httpClient.PostAsJsonAsync(endpoint, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
}


}
