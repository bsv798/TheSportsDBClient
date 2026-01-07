using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using TheSportsDBClient;

namespace TheSportsDbTests
{
    public class ResilientHttpClientTests
    {
        public ResilientHttpClientTests()
        {
            ResilientHttpClient.ResetPolly();
        }

        [Fact]
        public async Task Concurrent_Requests_Should_Not_Exceed_Rate_Limit()
        {
            // Arrange
            var requestCount = 0;
            var concurrentRequests = 0;
            var maxConcurrent = 0;
            var lockObj = new object();

            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                lock (lockObj)
                {
                    concurrentRequests++;
                    maxConcurrent = Math.Max(maxConcurrent, concurrentRequests);
                }

                Thread.Sleep(10);

                lock (lockObj)
                {
                    concurrentRequests--;
                    requestCount++;
                }

                return (HttpStatusCode.OK, "{\"success\":true}");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            // Act
            var tasks = Enumerable.Range(0, 40)
                .Select(i => Task.Run(async () =>
                    await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://test-{i}"), CancellationToken.None)))
                .ToArray();

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(40, requestCount);
            Assert.Equal(1, maxConcurrent);
        }
        
        [Fact(Skip = "Disabled due to Polly.RateLimit.LockFreeTokenBucketRateLimiter implementation specifics")]
        public async Task RateLimit_Should_Block_21st_to25th_Requests_Until_Window_Slides()
        {
            // Arrange
            var requestTimes = new List<DateTime>();
            var lockObj = new object();

            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                lock (lockObj)
                {
                    requestTimes.Add(DateTime.UtcNow);
                }

                return (HttpStatusCode.OK, "{\"success\":true}");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            // Act
            var tasks = Enumerable.Range(0, 25)
                .Select(i => resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://test-{i}"), CancellationToken.None))
                .ToArray();

            await Task.WhenAll(tasks);

            requestTimes.Sort();

            // Assert
            Assert.Equal(25, requestTimes.Count);
            Assert.InRange((requestTimes[19] - requestTimes[0]).TotalSeconds, 0, 5);
            Assert.InRange((requestTimes[24] - requestTimes[0]).TotalSeconds, 50, 65);
        }

        [Fact]
        public async Task RateLimit_Should_Limit_20_Requests_Per_Minute()
        {
            // Arrange
            var requestTimes = new List<DateTime>();
            var lockObj = new object();
            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                lock (lockObj)
                {
                    requestTimes.Add(DateTime.UtcNow);
                }

                return (HttpStatusCode.OK, "{\"success\":true}");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            var tasks = Enumerable.Range(0, 40)
                .Select(i => resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://test-{i}"), CancellationToken.None))
                .ToArray();

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(40, requestTimes.Count);
            Assert.InRange((requestTimes[19] - requestTimes[0]).TotalSeconds, 57, 63);
            Assert.InRange((requestTimes[39] - requestTimes[0]).TotalSeconds, 117, 123);
        }

        [Fact]
        public async Task RateLimit_Should_Limit_20_Requests_Per_Minute_Sequential()
        {
            // Arrange
            var requestTimes = new List<DateTime>();
            var lockObj = new object();
            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                lock (lockObj)
                {
                    requestTimes.Add(DateTime.UtcNow);
                }

                return (HttpStatusCode.OK, "{\"success\":true}");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            for (int i = 0; i < 40; i++)
            {
                await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://test-{i}"), CancellationToken.None);

                Thread.Sleep(1000);
            }

            // Assert
            Assert.Equal(40, requestTimes.Count);
            Assert.InRange((requestTimes[19] - requestTimes[0]).TotalSeconds, 57, 63);
            Assert.InRange((requestTimes[39] - requestTimes[0]).TotalSeconds, 117, 123);
        }

        [Fact]
        public async Task RateLimit_Should_Work_Across_Multiple_Instances()
        {
            // Arrange
            var requestCount = 0;
            var mockHandler1 = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref requestCount);
                return (HttpStatusCode.OK, "{\"instance\":1}");
            });

            var mockHandler2 = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref requestCount);
                return (HttpStatusCode.OK, "{\"instance\":2}");
            });

            var resilientHttpClient1 = new ResilientHttpClient(mockHandler1.Object);
            var resilientHttpClient2 = new ResilientHttpClient(mockHandler2.Object);

            var stopwatch = Stopwatch.StartNew();

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(resilientHttpClient1.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://test-{i}"), CancellationToken.None));
                tasks.Add(resilientHttpClient2.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://test-{i}"), CancellationToken.None));
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            // Assert
            Assert.Equal(20, requestCount);
            Assert.InRange(stopwatch.ElapsedMilliseconds, 57000, 67000);
        }

        [Fact]
        public async Task Request_Should_Throw_Json_Parse_Exception()
        {
            // Arrange
            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                return (HttpStatusCode.OK, "Not a json");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);
            var tsdbClient = new TheSportsDBClientV1("123", resilientHttpClient);

            // Act
            Func<Task<LeaguesResponse>> act = async () =>
            {
                try
                {
                    return await tsdbClient.GetLeagueByIdAsync(4444);
                } catch (ApiException ex)
                {
                    throw ex.InnerException;
                }
            };

            // Assert
            await Assert.ThrowsAnyAsync<JsonException>(act);
        }

        [Fact]
        public async Task Retry_Should_Not_Retry_On_System_InvalidOperationException()
        {
            // Arrange
            var attempts = 0;

            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref attempts);

                return (HttpStatusCode.OK, "");
            });
            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            // Act
            Func<Task<HttpResponseMessage>> act = async () => await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "test"), CancellationToken.None);

            // Assert
            await Assert.ThrowsAnyAsync<InvalidOperationException>(act);
            Assert.Equal(0, attempts);
        }

        [Fact]
        public async Task Retry_Should_Not_Retry_When_Not_Found()
        {
            // Arrange
            var attempts = 0;

            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref attempts);

                return (HttpStatusCode.NotFound, "");
            });
            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            // Act
            await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://test"), CancellationToken.None);

            // Assert
            Assert.Equal(1, attempts);
        }

        [Fact]
        public async Task Retry_Should_Retry_Twice_With_Power_of_2_Seconds_Delay()
        {
            // Arrange
            var attempts = 0;
            var attemptTimes = new List<DateTime>();

            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref attempts);
                attemptTimes.Add(DateTime.UtcNow);

                if (attempts < 3)
                {
                    return (HttpStatusCode.InternalServerError, "");
                }

                return (HttpStatusCode.OK, "{\"success\":true}");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            // Act
            var result = await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://test"), CancellationToken.None);

            // Assert
            Assert.Equal(3, attempts);
            Assert.NotNull(result);

            Assert.InRange((attemptTimes[1] - attemptTimes[0]).TotalSeconds, 1.9, 2.1);
            Assert.InRange((attemptTimes[2] - attemptTimes[1]).TotalSeconds, 3.9, 4.1);
        }

        [Fact]
        public async Task Retry_Should_Return_Result_On_Second_Attempt()
        {
            // Arrange
            var attempts = 0;
            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref attempts);

                if (attempts == 1)
                {
                    return (HttpStatusCode.ServiceUnavailable, "");
                }

                return (HttpStatusCode.OK, "success");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            // Act
            var result = await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://test"), CancellationToken.None);

            // Assert
            Assert.Equal(2, attempts);
            Assert.Equal("success", await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Retry_Should_Stop_After_Max_Retries()
        {
            // Arrange
            var attempts = 0;
            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref attempts);

                return (HttpStatusCode.InternalServerError, "error");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            // Act
            var result = await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://test"), CancellationToken.None);

            // Assert
            Assert.Equal(3, attempts);
            Assert.Equal("error", await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Successful_Request_Should_Not_Retry()
        {
            // Arrange
            var attempts = 0;
            var mockHandler = TestHelper.CreateMockHttpHandler(() =>
            {
                Interlocked.Increment(ref attempts);

                return (HttpStatusCode.OK, "test");
            });

            var resilientHttpClient = new ResilientHttpClient(mockHandler.Object);

            var stopwatch = Stopwatch.StartNew();

            // Act
            var result = await resilientHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://test"), CancellationToken.None);

            stopwatch.Stop();

            // Assert
            Assert.Equal(1, attempts);
            Assert.Equal("test", await result.Content.ReadAsStringAsync());
            Assert.InRange(stopwatch.ElapsedMilliseconds, 0, 500);
        }
    }
}
