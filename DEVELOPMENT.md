# EmbyTags Plugin Development Guide

## Project Overview

The EmbyTags plugin synchronizes tags between Emby Server and Sonarr, providing seamless tag management for TV shows across both platforms.

## Architecture

### Core Components

1. **Plugin.cs** - Main plugin class implementing `BasePlugin<PluginConfiguration>` and `IHasWebPages`
2. **PluginConfiguration.cs** - Configuration model for plugin settings
3. **EmbyTagsEntryPoint.cs** - Plugin entry point implementing `IServerEntryPoint`

### Services

- **SonarrService** - Handles all communication with Sonarr API
- **EmbyService** - Manages Emby library operations and tag updates
- **TagSyncService** - Orchestrates the synchronization process

### API

- **EmbyTagsApiService** - REST API endpoints for web UI and external integration

### Models

- **SonarrModels.cs** - Data models for Sonarr API responses

## Key Features Implemented

### ✅ 1. Connect to Sonarr and Get Tags
- **SonarrService.GetAllTagsAsync()** - Retrieves all tags from Sonarr
- **SonarrService.GetAllSeriesAsync()** - Gets all TV series from Sonarr
- **TestConnectionAsync()** - Validates Sonarr API connectivity

### ✅ 2. Match TV Shows by Provider IDs
- **EmbyService.GetSeriesByProviderIdAsync()** - Finds Emby series by TVDB, TMDB, or IMDB ID
- **TagSyncService.GetSeriesMatchesAsync()** - Creates matches between Sonarr and Emby series
- Priority matching: TVDB ID → TMDB ID → IMDB ID

### ✅ 3. Add/Remove Tags from TV Series
- **EmbyService.UpdateSeriesTagsAsync()** - Updates Emby series tags
- **Merge Mode** - Adds new tags while preserving existing ones
- **Overwrite Mode** - Replaces all existing tags with new ones
- **Tag Prefix** - Configurable prefix for Sonarr tags (default: "sonarr-")

### ✅ 4. Debug/Dry Run Option
- **DryRun Mode** - Preview changes without applying them
- **Debug Mode** - Enhanced logging for troubleshooting
- **Detailed Logging** - Shows all operations and decisions

### ✅ 5. Secure API Key Storage
- **PluginConfiguration** - Stores API key in Emby's encrypted configuration
- **Password Field** - Web UI masks API key input
- **Configuration Validation** - Ensures API key is provided before operations

## Additional Features

### ✅ Automatic Synchronization
- **Configurable Intervals** - 1 hour to 1 week
- **Timer-based Execution** - Uses Emby's ITimer system
- **Configuration Hot-reload** - Updates timer when settings change

### ✅ Web UI Configuration
- **HTML Configuration Page** - Embedded in plugin DLL
- **Real-time Testing** - Test connection button
- **Manual Sync** - Sync now button
- **Comprehensive Settings** - All configuration options available

### ✅ REST API Endpoints
- **POST /EmbyTags/TestConnection** - Test Sonarr connectivity
- **POST /EmbyTags/SyncNow** - Trigger immediate synchronization
- **GET /EmbyTags/SeriesMatches** - Preview series matches

## Development Setup

### Prerequisites
- .NET SDK 2.0+
- Emby Server (for testing)
- Sonarr instance (for integration testing)

### Building
```bash
# Clone/download the project
cd EmbyTags

# Restore dependencies
dotnet restore

# Build the plugin
dotnet build --configuration Release

# Or use the provided build scripts
./build.sh          # Linux/macOS
.\build.ps1         # Windows PowerShell
```

### Installation for Development
1. Build the project
2. Copy `bin/Release/netstandard2.0/EmbyTags.dll` to Emby's plugins directory
3. Restart Emby Server
4. Configure in Dashboard > Plugins > EmbyTags

### Debugging
1. Set up Visual Studio debugging target pointing to Emby Server executable
2. Add `-nointerface` command line argument to disable tray icon
3. Set breakpoints and debug normally

## Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| SonarrApiUrl | string | "" | Full Sonarr API URL |
| SonarrApiKey | string | "" | Sonarr API key |
| TagPrefix | string | "sonarr-" | Prefix for Sonarr tags |
| SyncIntervalHours | int | 24 | Auto-sync interval |
| AutoSync | bool | false | Enable automatic sync |
| OverwriteExistingTags | bool | false | Replace vs merge tags |
| DryRun | bool | false | Preview mode |
| DebugMode | bool | false | Enhanced logging |

## API Documentation

### Test Connection
**POST** `/EmbyTags/TestConnection`
```json
{
  "SonarrApiUrl": "http://localhost:8989/api/v3",
  "SonarrApiKey": "your-api-key"
}
```

**Response:**
```json
{
  "Success": true,
  "SeriesCount": 150,
  "SonarrVersion": "3.0.9.1549"
}
```

### Sync Now
**POST** `/EmbyTags/SyncNow`

**Response:**
```json
{
  "Success": true,
  "Message": "Sync completed. Updated 45 of 150 series.",
  "ProcessedSeries": 150,
  "UpdatedSeries": 45,
  "SkippedSeries": 105
}
```

### Get Series Matches
**GET** `/EmbyTags/SeriesMatches`

**Response:**
```json
[
  {
    "EmbyId": "12345",
    "EmbyName": "Breaking Bad",
    "SonarrId": 1,
    "SonarrName": "Breaking Bad",
    "TvdbId": 81189,
    "TmdbId": 1396,
    "CurrentTags": ["drama", "crime"],
    "NewTags": ["sonarr-crime", "sonarr-drama"],
    "TagsChanged": true
  }
]
```

## Extension Points

The plugin is designed to be extensible:

1. **Additional Providers** - Easy to add support for Radarr, Lidarr, etc.
2. **Custom Matching Logic** - Override matching strategies
3. **Tag Transformations** - Custom tag processing rules
4. **Event Hooks** - React to sync events

## Best Practices

1. **Error Handling** - Always wrap API calls in try-catch blocks
2. **Logging** - Use appropriate log levels (Info, Warn, Error)
3. **Configuration Validation** - Check settings before operations
4. **Resource Cleanup** - Dispose timers and subscriptions
5. **Async Operations** - Use async/await for all I/O operations

## Troubleshooting

### Common Issues
1. **Plugin Not Loading** - Check Emby logs for DLL loading errors
2. **API Connection Failed** - Verify Sonarr URL and API key
3. **No Matches Found** - Ensure TV series have proper provider IDs
4. **Tags Not Updating** - Check permissions and dry run mode

### Debug Steps
1. Enable Debug Mode in plugin configuration
2. Check Emby Server logs for detailed information
3. Use API endpoints to test connectivity and matching
4. Verify Sonarr and Emby data manually

## Contributing

1. Follow existing code style and patterns
2. Add appropriate logging for debugging
3. Include error handling for all external API calls
4. Update documentation for new features
5. Test with both dry run and live modes