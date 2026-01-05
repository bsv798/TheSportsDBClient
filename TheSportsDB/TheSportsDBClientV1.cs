namespace TheSportsDB
{
    public class TheSportsDBClientV1 : InternalOpenapiV1Client
    {
        public static readonly string DefaultApiKey = "123";

        public TheSportsDBClientV1(HttpClient httpClient) : this(DefaultApiKey, httpClient)
        {
        }

        public TheSportsDBClientV1(string apiKey, HttpClient httpClient) : base(httpClient)
        {
            BaseUrl = BaseUrl.Replace("{api-key}", apiKey);
        }
    }
}
