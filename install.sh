#!/bin/bash

# EmbyTags Plugin Installation Script

echo "EmbyTags Plugin Installer"
echo "========================="

# Function to detect OS
detect_os() {
    if [[ "$OSTYPE" == "darwin"* ]]; then
        echo "macOS"
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        echo "Linux"
    elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
        echo "Windows"
    else
        echo "Unknown"
    fi
}

# Function to find Emby plugins directory
find_emby_plugins_dir() {
    local os=$(detect_os)
    
    case $os in
        "macOS")
            echo "$HOME/Library/Application Support/Emby-Server/programdata/plugins"
            ;;
        "Linux")
            if [[ -d "/var/lib/emby/plugins" ]]; then
                echo "/var/lib/emby/plugins"
            elif [[ -d "$HOME/.config/emby-server/plugins" ]]; then
                echo "$HOME/.config/emby-server/plugins"
            else
                echo "/var/lib/emby/plugins"
            fi
            ;;
        "Windows")
            echo "$APPDATA/Emby-Server/programdata/plugins"
            ;;
        *)
            echo ""
            ;;
    esac
}

# Build the plugin
echo "Building EmbyTags plugin..."
if ! dotnet build --configuration Release; then
    echo "Error: Build failed!"
    exit 1
fi

echo "Build completed successfully!"

# Find the DLL
DLL_PATH="bin/Release/netstandard2.0/EmbyTags.dll"
if [[ ! -f "$DLL_PATH" ]]; then
    echo "Error: Plugin DLL not found at $DLL_PATH"
    exit 1
fi

# Find Emby plugins directory
PLUGINS_DIR=$(find_emby_plugins_dir)

if [[ -z "$PLUGINS_DIR" ]]; then
    echo "Could not automatically detect Emby plugins directory."
    echo "Please manually copy $DLL_PATH to your Emby plugins directory:"
    echo "  - Windows: %AppData%\\Emby-Server\\programdata\\plugins\\"
    echo "  - Linux: /var/lib/emby/plugins/"
    echo "  - macOS: ~/Library/Application Support/Emby-Server/programdata/plugins/"
    echo "  - Docker: /config/plugins/"
    exit 1
fi

echo "Detected Emby plugins directory: $PLUGINS_DIR"

# Create plugins directory if it doesn't exist
if [[ ! -d "$PLUGINS_DIR" ]]; then
    echo "Creating plugins directory: $PLUGINS_DIR"
    mkdir -p "$PLUGINS_DIR"
fi

# Copy the plugin
echo "Installing EmbyTags.dll to $PLUGINS_DIR"
if cp "$DLL_PATH" "$PLUGINS_DIR/"; then
    echo "Plugin installed successfully!"
    echo ""
    echo "Next steps:"
    echo "1. Restart Emby Server"
    echo "2. Go to Dashboard > Plugins"
    echo "3. Configure EmbyTags plugin"
    echo "4. Enter your Sonarr API URL and key"
    echo "5. Test the connection and sync!"
else
    echo "Error: Failed to copy plugin to $PLUGINS_DIR"
    echo "You may need to run this script with elevated permissions (sudo)"
    exit 1
fi