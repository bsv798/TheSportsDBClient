using HtmlAgilityPack;
using System.Web;

namespace TheSportsDBClient
{
    public class TheSportsDBClientV1 : InternalOpenapiV1Client
    {
        public static readonly string RootUrl = "https://www.thesportsdb.com";
        public static readonly string DefaultApiKey = "123";

        public HttpClient HttpClient { get; }

        public TheSportsDBClientV1() : this(DefaultApiKey, new ResilientHttpClient())
        {
        }

        public TheSportsDBClientV1(string apiKey) : this(apiKey, new ResilientHttpClient())
        {
        }

        public TheSportsDBClientV1(HttpClient httpClient) : this(DefaultApiKey, httpClient)
        {
        }

        public TheSportsDBClientV1(string apiKey, HttpClient httpClient) : base(httpClient)
        {
            BaseUrl = $"{RootUrl}/api/v1/json/{apiKey}";
            HttpClient = httpClient;
        }

        private async Task<HtmlDocument> GetHtmlDocumentAsync(string urlTemplate, List<object> pathParams, Dictionary<string, object> queryParams)
        {
            var uriBuilder = new UriBuilder(string.Format(urlTemplate, pathParams.ToArray()))
            {
                Query = string.Join("&", queryParams.Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode((string)kv.Value)}"))
            };
            var html = await HttpClient.GetStringAsync(uriBuilder.Uri).ConfigureAwait(false);
            var document = new HtmlDocument();

            document.LoadHtml(html);

            return document;
        }

        public async Task<SeasonsResponse> GetSeason(int leagueId, string seasonName)
        {
            var document = await GetHtmlDocumentAsync($"{RootUrl}/season/{{0}}/{{1}}", [leagueId, seasonName], []);
            var season = new SeasonSchema();
            var response = new SeasonsResponse
            {
                Seasons = [season]
            };

            season.StrSeason = seasonName;
            season.StrPoster = document.DocumentNode.SelectSingleNode("//*[@id=\"feature\"]/div[@class=\"container\"]/div[@class=\"row\"]/div[@class=\"col-sm-9\"]/div[1]/text()")?.InnerText.Trim();
            season.StrBadge = document.DocumentNode.SelectSingleNode("//*[@id=\"feature\"]/div[@class=\"container\"]/div[@class=\"row\"]/div[@class=\"col-sm-3\"]/img")?.Attributes["src"]?.Value.Trim();

            return response;
        }
    }
}
