using Service.Clients;
using Service.Interfaces;

namespace Service.Tokens;

public class TokenProvider : ITokenProvider
{
    private static string Token { get; set; } = "invalid-token";

    private readonly HttpClient _httpClient;
    private readonly Policies _policies;
    private readonly ILogger<TokenProvider> _logger;

    public TokenProvider(HttpClient httpClient, Policies policies, ILogger<TokenProvider> logger)
    {
        _httpClient = httpClient;
        _policies = policies;
        _logger = logger;
    }

    public async Task<string> GetToken()
    {
        _logger.LogInformation($"Get token = {Token}");

        if (string.IsNullOrEmpty(Token))
        {
            await RefreshToken();
        }
        return Token;
    }

    public async Task RefreshToken()
    {
        var uri = "get-token";

        _logger.LogInformation($"Calling 3rd party API {_httpClient.BaseAddress}{uri}...");

        var response = await _policies.WaitAndRetry.ExecuteAsync(() => _httpClient.GetAsync(uri));
        if (response != null)
        {
            Token = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"New token = {Token}");
        }
    }
}