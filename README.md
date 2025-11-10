# Tag Sync Plugin for Emby

A powerful Emby Server plugin that automatically synchronizes tags between Sonarr and Emby for TV shows.

![Plugin Version](https://img.shields.io/badge/version-1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET-netstandard2.0-purple)
![License](https://img.shields.io/badge/license-MIT-green)

## Features

- **🔄 Automatic Synchronization**: Sync tags between Sonarr and Emby on a configurable schedule
- **🎯 Smart Matching**: Matches TV shows using TVDB, TMDB, or IMDB IDs
- **🏷️ Flexible Tagging**: Optional tag prefix for organization
- **🔍 Dry Run Mode**: Preview changes before applying them
- **⚙️ Easy Configuration**: Web-based configuration page in Emby dashboard
- **📊 Detailed Logging**: Comprehensive logging for troubleshooting
- **🔒 Secure**: API keys stored securely in Emby database

## Installation

### Download

Download the latest release from the [Releases](https://github.com/ellisd4/Emby-Tag-Sync-Plugin/releases) page.

### Manual Installation

1. Download `EmbyTags.dll` from the latest release
2. Copy it to your Emby Server plugins directory:
   - **Windows**: `%AppData%\Emby-Server\programdata\plugins\`
   - **Linux**: `/var/lib/emby/plugins/`
   - **macOS**: `~/Library/Application Support/Emby-Server/programdata/plugins/`
   - **Docker**: `/config/plugins/`
3. Restart Emby Server
4. Navigate to Dashboard > Plugins > Tag Sync to configure

### Build from Source

```bash
git clone https://github.com/ellisd4/Emby-Tag-Sync-Plugin.git
cd Emby-Tag-Sync-Plugin
./build.sh  # or build.ps1 on Windows
```

## Configuration

### Required Settings

- **Sonarr API URL**: Full URL to your Sonarr API (e.g., `https://sonarr.example.com/api/v3`)
- **Sonarr API Key**: Your Sonarr API key (found in Sonarr Settings > General)

### Optional Settings

- **Tag Prefix**: Prefix added to Sonarr tags in Emby (leave empty for no prefix)
- **Sync Interval**: How often to automatically sync (1-168 hours)
- **Auto Sync**: Enable automatic synchronization
- **Overwrite Existing Tags**: Replace all tags instead of merging
- **Dry Run Mode**: Preview changes without applying them
- **Debug Mode**: Enable detailed logging

## Usage

### Initial Setup

1. Configure Sonarr API URL and API Key
2. Click **Save** (this automatically tests the connection)
3. Check Emby logs to verify connection success
4. Enable **Dry Run Mode** for your first sync
5. Click **Sync Now** to preview changes
6. Review the logs to see what would be synced
7. Disable **Dry Run Mode** and sync again to apply changes

### Automatic Sync

1. Enable **Auto Sync** in configuration
2. Set desired **Sync Interval**
3. Tags will automatically sync at the specified interval

## How It Works

1. **Connection**: Plugin connects to Sonarr using provided API credentials
2. **Data Retrieval**: Fetches all TV series and tags from Sonarr
3. **Matching**: Matches Sonarr series to Emby series using:
   - TVDB ID (primary)
   - TMDB ID (secondary)
4. **Tag Processing**: Converts Sonarr tag IDs to names and applies configured prefix
5. **Synchronization**: Updates Emby series tags based on configuration

## Troubleshooting

### Connection Failed

- Verify Sonarr URL is correct (include `/api/v3` or not, plugin handles both)
- Check API key is valid
- Ensure Sonarr is accessible from Emby server
- Check Emby logs for detailed error messages

### No Series Matched

- Ensure TV series have proper TVDB/TMDB IDs in both Emby and Sonarr
- Run metadata refresh in Emby if needed
- Check logs for specific matching issues

### Tags Not Updating

- Verify Dry Run mode is disabled
- Check Emby has write permissions to its database
- Review logs for any error messages

### Debug Steps

1. Enable **Debug Mode** in plugin configuration
2. Click **Save** to test connection
3. Check Emby Server logs for detailed information
4. Use **Sync Now** with **Dry Run** enabled to preview
5. Review logs for matching and tag application details

## API Endpoints

The plugin provides REST API endpoints:

- `GET /EmbyTags/Test` - Test Sonarr connection
- `POST /EmbyTags/Sync?DryRun=true` - Sync tags (with dry run option)
- `GET /EmbyTags/Status` - Get plugin status

## Development

### Prerequisites

- .NET SDK 2.0 or later
- Visual Studio, Rider, or VS Code
- Emby Server for testing

### Building

```bash
dotnet build --configuration Release
```

### Testing

```bash
./install.sh  # Builds and installs to local Emby
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.

## Support

For issues and feature requests, please use the [GitHub issue tracker](https://github.com/ellisd4/Emby-Tag-Sync-Plugin/issues).

## Acknowledgments

- Built for the Emby Server platform
- Integrates with Sonarr for tag management
