using System.Net.Http.Headers;
using MediLabo.Common;
using System.Text.Json;

namespace MediLabo.Web.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AddAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        AddAuthorizationHeader();
        _logger.LogInformation("GET request to {Endpoint}", endpoint);

        var response = await _httpClient.GetAsync(endpoint);
        return await ProcessApiResponse<T>(response, endpoint, "GET");
    }

    public async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        AddAuthorizationHeader();
        _logger.LogInformation("POST request to {Endpoint}", endpoint);

        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        return await ProcessApiResponse<TResponse>(response, endpoint, "POST");
    }

    public async Task<Result<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        AddAuthorizationHeader();
        _logger.LogInformation("PUT request to {Endpoint}", endpoint);

        var response = await _httpClient.PutAsJsonAsync(endpoint, data);
        return await ProcessApiResponse<TResponse>(response, endpoint, "PUT");
    }

    public async Task<Result<bool>> DeleteAsync(string endpoint)
    {
        AddAuthorizationHeader();
        _logger.LogInformation("DELETE request to {Endpoint}", endpoint);

        var response = await _httpClient.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
            _logger.LogWarning("DELETE request to {Endpoint} failed: {Error}", endpoint, errorMsg);
            return Result<bool>.Failure(errorMsg);
        }

        _logger.LogInformation("DELETE request to {Endpoint} succeeded", endpoint);
        return Result<bool>.Success(true);
    }

    private async Task<Result<T>> ProcessApiResponse<T>(HttpResponseMessage response, string endpoint, string method)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content))
        {
            var errorMsg = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
            _logger.LogWarning("{Method} request to {Endpoint} failed: {Error}", method, endpoint, errorMsg);
            return Result<T>.Failure(errorMsg);
        }

        var result = JsonSerializer.Deserialize<Result<T>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
        {
            _logger.LogWarning("{Method} request to {Endpoint} returned null result", method, endpoint);
            return Result<T>.Failure("No data returned from API");
        }

        if (result.IsFailure)
        {
            _logger.LogWarning("{Method} request to {Endpoint} returned failure: {Error}", method, endpoint, result.Error);
        }
        else
        {
            _logger.LogInformation("{Method} request to {Endpoint} succeeded", method, endpoint);
        }

        return result;
    }
}