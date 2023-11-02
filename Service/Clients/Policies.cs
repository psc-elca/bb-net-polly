using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Service.Interfaces;
using System.Net;

namespace Service.Clients
{
    public class ExpiredTokenException : Exception { }

    public class Policies
    {
        public AsyncTimeoutPolicy Timeout { get; }
        public AsyncRetryPolicy<HttpResponseMessage> Retry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> WaitAndRetry { get; }
        public AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreaker { get; }

        private readonly ILogger<Policies> _logger;

        public Policies(ILogger<Policies> logger)
        {
            _logger = logger;

            Timeout = Policy.TimeoutAsync(TimeSpan.FromSeconds(1), TimeoutStrategy.Pessimistic);

            Retry = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => r.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
                .RetryAsync(3,
                    onRetry: (ex, retryCount, context) =>
                    {
                        _logger.LogInformation($"{(int)ex.Result.StatusCode}] ERROR -> Retry #{retryCount}...");
                    });

            WaitAndRetry = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => r.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2),
                    onRetry: (ex, delay, retryCount, context) =>
                    {
                        _logger.LogInformation($"[{(int)ex.Result.StatusCode}] ERROR -> Wait {delay.TotalSeconds}s -> Retry #{retryCount}...");
                    });

            CircuitBreaker = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => r.StatusCode is HttpStatusCode.ServiceUnavailable) // 503
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(10));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetTokenRefreshPolicy(IServiceProvider provider)
        {
            return Policy<HttpResponseMessage>
                .Handle<ExpiredTokenException>()
                .RetryAsync((_, _) =>
                {
                    provider.GetRequiredService<ITokenProvider>().RefreshToken().Wait(); // !!!
                });
        }
    }
}