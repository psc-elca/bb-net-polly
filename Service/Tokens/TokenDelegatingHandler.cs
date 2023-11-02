using Service.Clients;
using Service.Interfaces;
using System.Net;

namespace Service.Tokens;

public class TokenDelegatingHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<TokenDelegatingHandler> _logger;

    public TokenDelegatingHandler(ITokenProvider tokenProvider, ILogger<TokenDelegatingHandler> logger)
    {
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Inject Token
        request.Headers.Remove("X-TOKEN");
        request.Headers.Add("X-TOKEN", await _tokenProvider.GetToken());

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized) // 401
        {
            _logger.LogInformation("[401] -> ExpiredTokenException");
            throw new ExpiredTokenException();
        }
        return response;
    }
}