using Polly.CircuitBreaker;
using Service.Interfaces;

namespace Service.Clients
{
    public class ThirdPartyService : IThirdPartyService
    {
        private readonly HttpClient _httpClient;
        private readonly Policies _policies;
        private readonly ILogger<ThirdPartyService> _logger;

        public ThirdPartyService(HttpClient httpClient, Policies policies, ILogger<ThirdPartyService> logger)
        {
            _httpClient = httpClient;
            _policies = policies;
            _logger = logger;

            //_httpClient.Timeout = TimeSpan.FromSeconds(1);
        }

        public async Task<string> Get(string uri, CancellationToken cancellationToken)
        {
            // Uncomment for Circuit Breaker Demo
            //if (_policies.CircuitBreaker.CircuitState == CircuitState.Open)
            //{
            //    throw new Exception("[CB OPEN] Service unavailable -> blocking calls");
            //}

            _logger.LogInformation($"Calling 3rd party API {_httpClient.BaseAddress}{uri}...");

            // Uncomment for Circuit Breaker Demo
            //var wrappedPolicies = _policies.CircuitBreaker.WrapAsync(_policies.WaitAndRetry);
            //using HttpResponseMessage response = await wrappedPolicies.ExecuteAsync(() => _httpClient.GetAsync(uri, cancellationToken));
            
            var policy = _policies.WaitAndRetry;
            using HttpResponseMessage response = await policy.ExecuteAsync(() => _httpClient.GetAsync(uri, cancellationToken));

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