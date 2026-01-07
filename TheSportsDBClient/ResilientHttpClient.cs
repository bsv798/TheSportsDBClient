using Polly;
using Polly.RateLimit;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TheSportsDbTests")]

namespace TheSportsDBClient
{
    internal class ResilientHttpClient : HttpClient
    {
        private static AsyncPolicy RateLimiterPolicy;
        private static AsyncPolicy RateLimiterRetryPolicy;
        private static AsyncPolicy<HttpResponseMessage> ExceptionRetryPolicy;
        private static AsyncPolicy<HttpResponseMessage> CombinedPolicy;

        private readonly HttpClient _httpClient;

        static ResilientHttpClient()
        {
            ResetPolly();
        }

        public ResilientHttpClient(HttpClient httpClient) : this(httpClient, new HttpClientHandler())
        {
        }

        public ResilientHttpClient(HttpClient httpClient, HttpMessageHandler handler) : this(httpClient, handler, true)
        {
        }

        public ResilientHttpClient(HttpClient httpClient, HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
            _httpClient = httpClient;
        }

        public ResilientHttpClient(HttpMessageHandler handler) : this(handler, true)
        {
        }

        public ResilientHttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
            _httpClient = new HttpClient();
        }

        internal static void ResetPolly()
        {
            RateLimiterPolicy = Policy
                        .RateLimitAsync(20, TimeSpan.FromMinutes(1));
            RateLimiterRetryPolicy = Policy
                        .Handle<RateLimitRejectedException>()
                        .WaitAndRetryForeverAsync(
                            sleepDurationProvider: (attempt, exception, context) =>
                            {
                                return ((RateLimitRejectedException)exception).RetryAfter;
                            },
                            onRetryAsync: (exception, attempt, timespan, context) =>
                            {
                                //Console.WriteLine($"Rate limit exceeded. Waiting for {timespan.TotalSeconds:N1} seconds before retry attempt {attempt}.");

                                return Task.CompletedTask;
                            }
                        );
            ExceptionRetryPolicy = Policy
                        .Handle<HttpRequestException>()
                        .OrResult<HttpResponseMessage>(response =>
                        {
                            return !response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound;
                        })
                        .WaitAndRetryAsync(
                            retryCount: 2,
                            sleepDurationProvider: (attempt, exception, context) =>
                            {
                                return TimeSpan.FromSeconds(Math.Pow(2, attempt));
                            },
                            onRetryAsync: (exception, timespan, attempt, context) =>
                            {
                                return Task.CompletedTask;
                            }
                        );
            CombinedPolicy = RateLimiterRetryPolicy.WrapAsync(RateLimiterPolicy).WrapAsync(ExceptionRetryPolicy);
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return CombinedPolicy.ExecuteAsync(async () =>
            {
                return await base.SendAsync(CloneRequest(request), cancellationToken);
            });
        }

        private HttpRequestMessage CloneRequest(HttpRequestMessage request)
        {
            var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);

            foreach (var header in request.Headers)
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return newRequest;
        }
    }
}
