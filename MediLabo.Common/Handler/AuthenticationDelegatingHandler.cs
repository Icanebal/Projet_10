using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediLabo.Common.Handler;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationDelegatingHandler> _logger;

    public AuthenticationDelegatingHandler(IHttpContextAccessor httpContextAccessor,ILogger<AuthenticationDelegatingHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            _logger.LogWarning("HttpContext is null, cannot forward token to {RequestUri}", request.RequestUri);
            return await base.SendAsync(request, cancellationToken);
        }

        var authHeader = httpContext.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader))
        {
            _logger.LogDebug("Forwarding authentication token to {RequestUri}", request.RequestUri);

            if (request.Headers.Contains("Authorization"))
            {
                request.Headers.Remove("Authorization");
            }

            request.Headers.Add("Authorization", authHeader);
        }
        else
        {
            _logger.LogWarning("No authorization token found for request to {RequestUri}", request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
