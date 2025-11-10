# Build script for EmbyTags plugin (PowerShell)

Write-Host "Building EmbyTags Emby Plugin..." -ForegroundColor Green

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

# Build the project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build --configuration Release

# Check if build was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host "Plugin DLL location: bin\Release\netstandard2.0\EmbyTags.dll" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Installation Instructions:" -ForegroundColor Yellow
    Write-Host "1. Copy EmbyTags.dll to your Emby Server plugins directory:" -ForegroundColor White
    Write-Host "   - Windows: %AppData%\Emby-Server\programdata\plugins\" -ForegroundColor White
    Write-Host "   - Linux: /var/lib/emby/plugins/" -ForegroundColor White
    Write-Host "   - Docker: /config/plugins/" -ForegroundColor White
    Write-Host "2. Restart Emby Server" -ForegroundColor White
    Write-Host "3. Configure plugin in Dashboard > Plugins > EmbyTags" -ForegroundColor White
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}