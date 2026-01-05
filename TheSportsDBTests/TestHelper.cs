using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TheSportsDbTests
{
    public static class TestHelper
    {
        public static string LoadJsonFromEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fullResourceName = $"{assembly.GetName().Name}.TestData.{resourceName}";
            using var stream = assembly.GetManifestResourceStream(fullResourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Embedded resource не найден: {fullResourceName}");
            }

            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public static Mock<HttpMessageHandler> CreateMockHttpHandler(Func<(HttpStatusCode, string)> responseProvider)
        {
            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    var (statusCode, content) = responseProvider();

                    return new HttpResponseMessage(statusCode)
                    {
                        Content = new StringContent(content)
                    };
                });

            return mockHandler;
        }

        public static Mock<HttpMessageHandler> CreateMockHandlerWithJsonResource(HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    var code = statusCode;
                    var jsonResourceName = request.RequestUri.PathAndQuery.Split("/")[^1].Replace("&", "_").Replace("?", "_") + ".json";
                    var json = "";

                    try
                    {
                        json = LoadJsonFromEmbeddedResource(jsonResourceName);
                    }
                    catch (FileNotFoundException)
                    {
                        code = HttpStatusCode.NotFound;
                    }

                    return new HttpResponseMessage
                    {
                        StatusCode = code,
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                    };
                });

            return mockHandler;
        }
    }
}
