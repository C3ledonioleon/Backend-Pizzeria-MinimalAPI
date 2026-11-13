using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;

namespace MVC.Pizzeria.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("PizzeriaAPI");
            _httpContextAccessor = httpContextAccessor;
        }

        // Obtiene el JWT guardado en la autenticación por cookies.
        private async Task<HttpRequestMessage> CrearRequestAsync(
            HttpMethod metodo,
            string endpoint)
        {
            var request = new HttpRequestMessage(metodo, endpoint);

            var token = await _httpContextAccessor.HttpContext!
                .GetTokenAsync("Cookies", "access_token");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return request;
        }

        // GET
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            using var request = await CrearRequestAsync(
                HttpMethod.Get, endpoint);

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        // POST
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest request)
        {
            using var httpRequest = await CrearRequestAsync(
                HttpMethod.Post, endpoint);

            httpRequest.Content = JsonContent.Create(request);

            using var response = await _httpClient.SendAsync(httpRequest);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
    }
}