using MediLabo.Common;

namespace MediLabo.Web.Services;
public interface IApiService
{
    Task<Result<T>> GetAsync<T>(string endpoint);
    Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task<Result<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task<Result<bool>> DeleteAsync(string endpoint);
}
