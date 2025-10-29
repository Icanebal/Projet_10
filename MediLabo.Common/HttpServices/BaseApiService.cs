using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace MediLabo.Common.HttpServices;

public abstract class BaseApiService
{
    protected readonly HttpClient HttpClient;
    protected readonly ILogger Logger;

    protected BaseApiService(HttpClient httpClient, ILogger logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    protected async Task<Result<T>> GetAsync<T>(string endpoint, string logContext)
    {
            Logger.LogInformation("Calling GET {Endpoint} - {Context}", endpoint, logContext);

            var response = await HttpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("GET request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
                return Result<T>.Failure($"Erreur lors de l'appel à {endpoint}: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<Result<T>>();

            if (result == null)
            {
                Logger.LogWarning("Null response from {Endpoint}", endpoint);
                return Result<T>.Failure("Réponse vide ou invalide");
            }

            Logger.LogInformation("Successfully retrieved data from {Endpoint}", endpoint);

            return result;
    }
}
