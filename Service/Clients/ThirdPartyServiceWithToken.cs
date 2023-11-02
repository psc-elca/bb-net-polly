using Service.Interfaces;

namespace Service.Clients
{
    public class ThirdPartyServiceWithToken : IThirdPartyServiceWithToken
    {
        private readonly HttpClient _httpClient;
        private readonly Policies _policies;
        private readonly ILogger<ThirdPartyServiceWithToken> _logger;

        public ThirdPartyServiceWithToken(HttpClient httpClient, Policies policies, ILogger<ThirdPartyServiceWithToken> logger)
        {
            _httpClient = httpClient;
            _policies = policies;
            _logger = logger;
        }

        public async Task<string> Get(string uri, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Calling 3rd party API {_httpClient.BaseAddress}{uri}...");

            using HttpResponseMessage response = await _policies.WaitAndRetry.ExecuteAsync(() => _httpClient.GetAsync(uri, cancellationToken));
            
            if (response.IsSuccessStatusCode)
            {
                // Ok
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"[{(int)response.StatusCode}] OK -> {content}");
                return content;
            }
            else
            {
                // Error
                _logger.LogInformation($"[{(int)response.StatusCode}] FINAL ERROR");
                return "error"; //throw new HttpRequestException();
            }
        }
    }
}