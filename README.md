# EmbyTags Plugin

An Emby Server plugin that synchronizes tags between Emby and Sonarr for TV shows.

## Features

- **Sonarr Integration**: Connects to Sonarr API to retrieve TV show tags
- **Smart Matching**: Matches TV shows between Sonarr and Emby using TVDB, TMDB, or IMDB IDs
- **Tag Synchronization**: Adds or removes tags from TV series in Emby based on Sonarr tags
- **Debug Mode**: Dry run option to preview changes without making actual modifications
- **Secure Storage**: Stores Sonarr API key securely in the Emby database
- **Automatic Sync**: Optional scheduled synchronization at configurable intervals
- **Web UI**: Easy-to-use configuration page in the Emby dashboard

## Installation

1. Build the plugin:
   ```bash
   dotnet build EmbyTags.csproj
   ```

2. Copy the built DLL to your Emby Server plugins directory:
   - Windows: `%AppData%\Emby-Server\programdata\plugins\`
   - Linux: `/var/lib/emby/plugins/`
   - Docker: `/config/plugins/`

3. Restart Emby Server

4. Navigate to Dashboard > Plugins > EmbyTags to configure

## Configuration

### Required Settings
- **Sonarr API URL**: Full URL to your Sonarr API (e.g., `http://localhost:8989/api/v3`)
- **Sonarr API Key**: Your Sonarr API key (found in Sonarr Settings > General)

### Optional Settings
- **Tag Prefix**: Prefix added to Sonarr tags in Emby (default: `sonarr-`)
- **Sync Interval**: How often to automatically sync (1-168 hours)
- **Auto Sync**: Enable automatic synchronization
- **Overwrite Existing Tags**: Replace all tags instead of merging
- **Dry Run Mode**: Preview changes without applying them
- **Debug Mode**: Enable detailed logging

## How It Works

1. **Connection**: Plugin connects to Sonarr using the provided API URL and key
2. **Data Retrieval**: Fetches all TV series and their tags from Sonarr
3. **Matching**: Matches Sonarr series to Emby series using:
   - TVDB ID (primary)
   - TMDB ID (secondary)
   - IMDB ID (fallback)
4. **Tag Processing**: Converts Sonarr tag IDs to names and applies the configured prefix
5. **Synchronization**: Updates Emby series tags based on configuration settings

## API Endpoints

The plugin provides REST API endpoints for external integration:

- `POST /EmbyTags/TestConnection` - Test Sonarr connection
- `POST /EmbyTags/SyncNow` - Trigger immediate sync
- `GET /EmbyTags/SeriesMatches` - Get preview of series matches

## Logging

The plugin logs its activities to the Emby Server log. Enable Debug Mode in the configuration for detailed logging information.

## Troubleshooting

### Common Issues

1. **Connection Failed**: Verify Sonarr URL and API key are correct
2. **No Series Matched**: Ensure your TV series have proper TVDB/TMDB/IMDB IDs
3. **Tags Not Updating**: Check if Dry Run mode is enabled
4. **Permission Errors**: Ensure Emby has write access to its database

### Debug Steps

1. Enable Debug Mode in plugin configuration
2. Check Emby Server logs for detailed information
3. Use "Test Sonarr Connection" button to verify connectivity
4. Use "Sync Now" with Dry Run enabled to preview changes

## Development

### Prerequisites
- .NET SDK 2.0 or later
- Visual Studio or VS Code
- Emby Server for testing

### Building
```bash
dotnet build EmbyTags.csproj
```

### Testing
1. Copy the built DLL to your Emby plugins directory
2. Restart Emby Server
3. Configure the plugin and test functionality

## License

This project is licensed under the MIT License.

## Support

For issues and feature requests, please use the GitHub issue tracker.