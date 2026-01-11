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

### From NuGet.org (Recommended)

```bash
dotnet add package TheSportsDBClient
```

Or via NuGet Package Manager:

```
Install-Package TheSportsDBClient
```

### From GitHub Packages

GitHub Packages requires authentication even for public packages.

To setup system-wide access to repository:

1. Create a Personal Access Token (PAT) with `read:packages` scope at https://github.com/settings/tokens

2. Add GitHub Packages as a NuGet source:

```bash
dotnet nuget add source \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_PAT \
  --store-password-in-clear-text \
  --name github \
  "https://nuget.pkg.github.com/bsv798/index.json"
```

3. Install the package:

```bash
dotnet add package TheSportsDBClient --source github
```

To setup project-wide access to repository:

1. Create a `nuget.config` file in your project root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="github" value="https://nuget.pkg.github.com/bsv798/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="%GITHUB_USERNAME%" />
      <add key="ClearTextPassword" value="%GITHUB_PAT%" />
    </github>
  </packageSourceCredentials>
</configuration>
```

Note environment variables `GITHUB_USERNAME` and `GITHUB_PAT`.

2. Then install normally:

```bash
dotnet add package TheSportsDBClient
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
