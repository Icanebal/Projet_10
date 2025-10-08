using System.Net.Http.Headers;
using MediLabo.Common;

namespace MediLabo.Web.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiService(HttpClient httpClient,ILogger<ApiService> logger,IHttpContextAccessor httpContextAccessor)
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
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        AddAuthorizationHeader();

        _logger.LogInformation("GET request to {Endpoint}", endpoint);

        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
            _logger.LogWarning("GET request to {Endpoint} failed: {Error}", endpoint, errorMessage);
            return Result<T>.Failure(errorMessage);
        }

        T? data = await response.Content.ReadFromJsonAsync<T>();

        if (data == null)
        {
            _logger.LogWarning("GET request to {Endpoint} returned null data", endpoint);
            return Result<T>.Failure("No data returned from API");
        }

        _logger.LogInformation("GET request to {Endpoint} succeeded", endpoint);
        return Result<T>.Success(data);
    }

    public async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        AddAuthorizationHeader();

        _logger.LogInformation("POST request to {Endpoint}", endpoint);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(endpoint, data);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
            _logger.LogWarning("POST request to {Endpoint} failed: {Error}", endpoint, errorMessage);
            return Result<TResponse>.Failure(errorMessage);
        }

        TResponse? responseData = await response.Content.ReadFromJsonAsync<TResponse>();

        if (responseData == null)
        {
            _logger.LogWarning("POST request to {Endpoint} returned null data", endpoint);
            return Result<TResponse>.Failure("No data returned from API");
        }

        _logger.LogInformation("POST request to {Endpoint} succeeded", endpoint);
        return Result<TResponse>.Success(responseData);
    }

    public async Task<Result<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        AddAuthorizationHeader();

        _logger.LogInformation("PUT request to {Endpoint}", endpoint);

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(endpoint, data);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
            _logger.LogWarning("PUT request to {Endpoint} failed: {Error}", endpoint, errorMessage);
            return Result<TResponse>.Failure(errorMessage);
        }

        TResponse? responseData = await response.Content.ReadFromJsonAsync<TResponse>();

        if (responseData == null)
        {
            _logger.LogWarning("PUT request to {Endpoint} returned null data", endpoint);
            return Result<TResponse>.Failure("No data returned from API");
        }

        _logger.LogInformation("PUT request to {Endpoint} succeeded", endpoint);
        return Result<TResponse>.Success(responseData);
    }

    public async Task<Result<bool>> DeleteAsync(string endpoint)
    {
        AddAuthorizationHeader();

        _logger.LogInformation("DELETE request to {Endpoint}", endpoint);

        HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
            _logger.LogWarning("DELETE request to {Endpoint} failed: {Error}", endpoint, errorMessage);
            return Result<bool>.Failure(errorMessage);
        }

        _logger.LogInformation("DELETE request to {Endpoint} succeeded", endpoint);
        return Result<bool>.Success(true);
    }
}