using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using TheSportsDBClient;
using Xunit;

namespace TheSportsDbTests
{
    public class TheSportsDbClientTests
    {
        private readonly TheSportsDBClientV1 _client;

        public TheSportsDbClientTests()
        {
            ResilientHttpClient.InitializePolly();

            var mockHandler = TestHelper.CreateMockHandlerWithJsonResource();
            var httpClient = new ResilientHttpClient(mockHandler.Object);

            _client = new TheSportsDBClientV1(httpClient);
        }

        // ------------------ v1 API Search ------------------

        [Fact]
        public async Task GetTeamByNameAsync()
        {
            // Act
            var result = await _client.GetTeamByNameAsync("Arsenal");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventByTitleAsync()
        {
            // Act
            var result = await _client.GetEventByTitleAsync("Arsenal_vs_Chelsea", null, null, null);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPlayerByNameAsync()
        {
            // Act
            var result = await _client.GetPlayerByNameAsync("Danny_Welbeck");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetVenueByNameAsync()
        {
            // Act
            var result = await _client.GetVenueByNameAsync("Wembley");

            // Assert
            Assert.NotNull(result);
        }

        // ------------------ v1 API Lookup ------------------

        [Fact]
        public async Task GetLeagueByIdAsync()
        {
            // Act
            var result = await _client.GetLeagueByIdAsync(4328);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTableByIdAsync()
        {
            // Act
            var result = await _client.GetTableByIdAsync(4328, "2020-2021");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTeamByIdAsync()
        {
            // Act
            var result = await _client.GetTeamByIdAsync(133604);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEquipmentByIdAsync()
        {
            // Act
            var result = await _client.GetEquipmentByIdAsync(133597);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPlayerByIdAsync()
        {
            // Act
            var result = await _client.GetPlayerByIdAsync(34145937);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetHonourByIdAsync()
        {
            // Act
            var result = await _client.GetHonourByIdAsync(34147178);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetFormerTeamsByPlayerIdAsync()
        {
            // Act
            var result = await _client.GetFormerTeamsByPlayerIdAsync(34147178);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMilestonesByPlayerIdAsync()
        {
            // Act
            var result = await _client.GetMilestonesByPlayerIdAsync(34161397);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetContractsByPlayerIdAsync()
        {
            // Act
            var result = await _client.GetContractsByPlayerIdAsync(34147178);

            // Assert
            Assert.NotNull(result);
        }

        [Fact(Skip = "TODO Add method GetResultsByPlayerIdAsync")]
        public async Task GetResultsByPlayerIdAsync()
        {
            // Act
            var result = await _client.GetEventResultsAsync(0);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventByIdAsync()
        {
            // Act
            var result = await _client.GetEventByIdAsync(441613);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventResultsAsync()
        {
            // Act
            var result = await _client.GetEventResultsAsync(652890);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLineupByEventIdAsync()
        {
            // Act
            var result = await _client.GetLineupByEventIdAsync(1032723);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTimelineByEventIdAsync()
        {
            // Act
            var result = await _client.GetTimelineByEventIdAsync(1032718);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventStatsByIdAsync()
        {
            // Act
            var result = await _client.GetEventStatsByIdAsync(1032723);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTVByEventIdAsync()
        {
            // Act
            var result = await _client.GetTVByEventIdAsync(584911);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetVenueByIdAsync()
        {
            // Act
            var result = await _client.GetVenueByIdAsync(16163);

            // Assert
            Assert.NotNull(result);
        }

        // ------------------ v1 API List ------------------

        [Fact]
        public async Task GetAllSportsAsync()
        {
            // Act
            var result = await _client.GetAllSportsAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllCountriesAsync()
        {
            // Act
            var result = await _client.GetAllCountriesAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllLeaguesAsync()
        {
            // Act
            var result = await _client.GetAllLeaguesAsync();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SearchAllLeaguesAsync()
        {
            // Act
            var result = await _client.SearchAllLeaguesAsync("Soccer", "England");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SearchAllSeasonsAsync()
        {
            // Act
            var result = await _client.SearchAllSeasonsAsync("4328", null, 1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact(Skip = "TODO Add parameter description to method SearchAllSeasonsAsync")]
        public async Task SearchAllSeasonsAsyncDescription()
        {
            // Act
            var result = await _client.SearchAllSeasonsAsync("4328", null, null);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SearchAllTeamsAsync()
        {
            // Act
            var result = await _client.SearchAllTeamsAsync(null, "Spain", "Soccer");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task LookupAllPlayersAsync()
        {
            // Act
            var result = await _client.LookupAllPlayersAsync(133604);

            // Assert
            Assert.NotNull(result);
        }

        // ------------------ v1 API Schedule ------------------

        [Fact]
        public async Task GetNextEventsAsync()
        {
            // Act
            var result = await _client.GetNextEventsAsync(133602);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLastEventsAsync()
        {
            // Act
            var result = await _client.GetLastEventsAsync(133602);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetNextLeagueEventsAsync()
        {
            // Act
            var result = await _client.GetNextLeagueEventsAsync(4328);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPastLeagueEventsAsync()
        {
            // Act
            var result = await _client.GetPastLeagueEventsAsync(4328);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventsByDayAsync()
        {
            // Act
            var result = await _client.GetEventsByDayAsync(DateTimeOffset.Parse("2014-10-10"), "Baseball", null);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventsBySeasonAsync()
        {
            // Act
            var result = await _client.GetEventsBySeasonAsync(4328, "2014-2015");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetEventsByTVAsync()
        {
            // Act
            var result = await _client.GetEventsByTVAsync(DateTimeOffset.Parse("2018-07-07"), null, "Fighting");

            // Assert
            Assert.NotNull(result);
        }

        // ------------------ v1 API Video ------------------

        [Fact]
        public async Task GetEventHighlightsAsync()
        {
            // Act
            var result = await _client.GetEventHighlightsAsync(DateTimeOffset.Parse("2024-07-07"), null, "motorsport");

            // Assert
            Assert.NotNull(result);
        }

        // ------------------ v1 Extra ------------------

        [Fact]
        public async Task GetSeasonAsync()
        {
            // Act
            var mockHandler = TestHelper.CreateMockHandlerWithJsonResource("season_4346_2025.html");
            var httpClient = new ResilientHttpClient(mockHandler.Object);
            var client = new TheSportsDBClientV1(httpClient);
            var result = await client.GetSeason(4346, "2025");

            // Assert
            Assert.NotNull(result);
        }
    }
}
