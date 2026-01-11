# TheSportsDBClient

[![NuGet](https://img.shields.io/nuget/v/TheSportsDBClient.svg)](https://www.nuget.org/packages/TheSportsDBClient/)

Unofficial C# client library for [TheSportsDB API](https://www.thesportsdb.com/api.php). Provides easy access to sports data including teams, players, leagues, events, and more.

## Features

- Full coverage of TheSportsDB API endpoints
- Built-in retry logic with Polly
- Rate limiting support
- Strongly-typed responses
- Source Link enabled for easy debugging

## Installation

```bash
dotnet add package TheSportsDBClient
```

Or via NuGet Package Manager:

```
Install-Package TheSportsDBClient
```

## Quick Start

```csharp
using TheSportsDBClient;

// Initialize the client with your API key
var client = new TheSportsDBClient("your-api-key");

// Search for a team
var teams = await client.SearchTeamsAsync("Arsenal");

// Get league details
var league = await client.GetLeagueByIdAsync(4328);

// Lookup player information
var player = await client.SearchPlayersAsync("Messi");

// Get upcoming events
var events = await client.GetNextEventsAsync(133604);
```

## API Key

Get your free API key from [TheSportsDB](https://www.thesportsdb.com/api.php).

For development and testing, you can use the test API key: `123`
