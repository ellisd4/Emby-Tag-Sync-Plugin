#!/bin/bash

# Build script for EmbyTags plugin

echo "Building EmbyTags Emby Plugin..."

# Clean previous builds
echo "Cleaning previous builds..."
dotnet clean

# Restore packages
echo "Restoring NuGet packages..."
dotnet restore

# Build the project
echo "Building project..."
dotnet build --configuration Release

# Check if build was successful
if [ $? -eq 0 ]; then
    echo "Build successful!"
    echo "Plugin DLL location: bin/Release/netstandard2.0/EmbyTags.dll"
    echo ""
    echo "Installation Instructions:"
    echo "1. Copy EmbyTags.dll to your Emby Server plugins directory:"
    echo "   - Windows: %AppData%\\Emby-Server\\programdata\\plugins\\"
    echo "   - Linux: /var/lib/emby/plugins/"
    echo "   - Docker: /config/plugins/"
    echo "2. Restart Emby Server"
    echo "3. Configure plugin in Dashboard > Plugins > EmbyTags"
else
    echo "Build failed!"
    exit 1
fi