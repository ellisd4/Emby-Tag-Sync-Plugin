using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.Services;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Controller.Library;
using EmbyTags.Services;

namespace EmbyTags.Api
{
    [Route("/EmbyTags/Test", "GET", Summary = "Test Sonarr connection")]
    public class TestSonarrConnection : IReturn<object>
    {
    }

    [Route("/EmbyTags/Sync", "POST", Summary = "Sync tags from Sonarr to Emby")]
    public class SyncTags : IReturn<object>
    {
        public bool DryRun { get; set; } = false;
    }

    [Route("/EmbyTags/Status", "GET", Summary = "Get plugin status")]
    public class GetStatus : IReturn<object>
    {
    }

    public class SimpleEmbyTagsApiService : IService
    {
        private readonly ILogger _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILibraryManager _libraryManager;

        public SimpleEmbyTagsApiService(ILogger logger, IJsonSerializer jsonSerializer, ILibraryManager libraryManager)
        {
            _logger = logger;
            _jsonSerializer = jsonSerializer;
            _libraryManager = libraryManager;
        }

        public async Task<object> Get(TestSonarrConnection request)
        {
            try
            {
                _logger.Info("========================================");
                _logger.Info("TESTING SONARR CONNECTION");
                _logger.Info("========================================");
                
                var config = Plugin.Instance?.Configuration;
                if (config != null)
                {
                    _logger.Info($"Sonarr URL: {config.SonarrApiUrl}");
                    _logger.Info($"API Key configured: {!string.IsNullOrWhiteSpace(config.SonarrApiKey)}");
                }
                
                var sonarrService = new SimpleSonarrService(_logger, _jsonSerializer);
                var result = await sonarrService.TestConnectionAsync();
                
                if (result)
                {
                    _logger.Info("✓ CONNECTION TEST SUCCESSFUL");
                    _logger.Info("========================================");
                }
                else
                {
                    _logger.Error("✗ CONNECTION TEST FAILED");
                    _logger.Error("========================================");
                }
                
                return new { Success = result, Message = result ? "Connection successful" : "Connection failed" };
            }
            catch (Exception ex)
            {
                _logger.ErrorException("✗ ERROR DURING CONNECTION TEST", ex);
                _logger.Error("========================================");
                return new { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<object> Post(SyncTags request)
        {
            try
            {
                _logger.Info("========================================");
                _logger.Info($"STARTING TAG SYNC (DRY RUN: {request.DryRun})");
                _logger.Info("========================================");
                
                var config = Plugin.Instance?.Configuration;
                if (config == null)
                {
                    _logger.Error("Plugin configuration is null");
                    return new { Success = false, ErrorMessage = "Plugin not configured" };
                }
                
                _logger.Info($"Sonarr URL: {config.SonarrApiUrl}");
                _logger.Info($"Tag Prefix: {config.TagPrefix}");
                _logger.Info($"Overwrite Mode: {config.OverwriteExistingTags}");
                _logger.Info($"Dry Run Mode: {request.DryRun}");
                
                var sonarrService = new SimpleSonarrService(_logger, _jsonSerializer);
                var embyService = new SimpleEmbyService(_libraryManager, _logger);
                
                // Fetch Sonarr data
                _logger.Info("Fetching series from Sonarr...");
                var sonarrSeries = await sonarrService.GetAllSeriesAsync();
                _logger.Info($"✓ Retrieved {sonarrSeries.Count} series from Sonarr");
                
                _logger.Info("Fetching tags from Sonarr...");
                var sonarrTags = await sonarrService.GetAllTagsAsync();
                _logger.Info($"✓ Retrieved {sonarrTags.Count} tags from Sonarr");
                
                // Create tag ID to name mapping
                var tagMap = sonarrTags.ToDictionary(t => t.Id, t => t.Label);
                
                // Fetch Emby series
                _logger.Info("Fetching TV series from Emby...");
                var embySeries = embyService.GetAllTvSeries();
                _logger.Info($"✓ Retrieved {embySeries.Count} series from Emby");
                
                if (request.DryRun)
                {
                    _logger.Info("========================================");
                    _logger.Info("DRY RUN MODE - No changes will be made to Emby");
                    _logger.Info("========================================");
                }
                
                // Match and sync
                int matchedCount = 0;
                int tagsAddedCount = 0;
                int tagsRemovedCount = 0;
                
                _logger.Info("Starting series matching and tag sync...");
                
                foreach (var sonarrShow in sonarrSeries)
                {
                    var tvdbId = sonarrShow.TvdbId > 0 ? sonarrShow.TvdbId.ToString() : null;
                    var tmdbId = sonarrShow.TmdbId > 0 ? sonarrShow.TmdbId.ToString() : null;
                    
                    if (string.IsNullOrEmpty(tvdbId) && string.IsNullOrEmpty(tmdbId))
                    {
                        _logger.Debug($"Skipping Sonarr series '{sonarrShow.Title}' - no TVDB or TMDB ID");
                        continue;
                    }
                    
                    var embyShow = embyService.FindSeriesByExternalId(tvdbId, tmdbId);
                    if (embyShow == null)
                    {
                        _logger.Debug($"No Emby match found for Sonarr series '{sonarrShow.Title}' (TVDB: {tvdbId}, TMDB: {tmdbId})");
                        continue;
                    }
                    
                    matchedCount++;
                    
                    // Get tags for this show
                    var sonarrShowTags = sonarrShow.Tags?.Select(tagId => 
                    {
                        if (tagMap.TryGetValue(tagId, out var tagLabel))
                        {
                            return $"{config.TagPrefix}{tagLabel}";
                        }
                        return null;
                    }).Where(t => t != null).ToList() ?? new List<string>();
                    
                    var currentTags = embyShow.Tags?.ToList() ?? new List<string>();
                    var prefixedTags = currentTags.Where(t => t.StartsWith(config.TagPrefix, StringComparison.OrdinalIgnoreCase)).ToList();
                    
                    if (config.OverwriteExistingTags)
                    {
                        // Remove all prefixed tags and add new ones
                        foreach (var oldTag in prefixedTags)
                        {
                            if (!sonarrShowTags.Contains(oldTag, StringComparer.OrdinalIgnoreCase))
                            {
                                if (request.DryRun)
                                {
                                    _logger.Info($"[DRY RUN] Would remove tag '{oldTag}' from '{embyShow.Name}' (TVDB: {tvdbId})");
                                    tagsRemovedCount++;
                                }
                                else
                                {
                                    if (embyService.RemoveTagFromSeries(embyShow.Id, oldTag))
                                    {
                                        tagsRemovedCount++;
                                    }
                                }
                            }
                        }
                    }
                    
                    // Add new tags
                    foreach (var newTag in sonarrShowTags)
                    {
                        if (!currentTags.Contains(newTag, StringComparer.OrdinalIgnoreCase))
                        {
                            if (request.DryRun)
                            {
                                _logger.Info($"[DRY RUN] Would add tag '{newTag}' to '{embyShow.Name}' (TVDB: {tvdbId})");
                                tagsAddedCount++;
                            }
                            else
                            {
                                if (embyService.AddTagToSeries(embyShow.Id, newTag))
                                {
                                    tagsAddedCount++;
                                }
                            }
                        }
                    }
                }
                
                _logger.Info("========================================");
                _logger.Info("SYNC COMPLETED SUCCESSFULLY");
                _logger.Info($"Matched: {matchedCount} series");
                _logger.Info($"Tags added: {tagsAddedCount}");
                _logger.Info($"Tags removed: {tagsRemovedCount}");
                _logger.Info("========================================");
                
                return new { 
                    Success = true, 
                    Message = $"Matched {matchedCount} series, added {tagsAddedCount} tags, removed {tagsRemovedCount} tags",
                    DryRun = request.DryRun,
                    SonarrSeriesCount = sonarrSeries.Count,
                    SonarrTagCount = sonarrTags.Count,
                    EmbySeriesCount = embySeries.Count,
                    MatchedCount = matchedCount,
                    TagsAdded = tagsAddedCount,
                    TagsRemoved = tagsRemovedCount
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorException("✗ ERROR DURING TAG SYNC", ex);
                _logger.Error("========================================");
                return new { Success = false, ErrorMessage = ex.Message };
            }
        }

        public object Get(GetStatus request)
        {
            try
            {
                var config = Plugin.Instance?.Configuration;
                return new
                {
                    IsConfigured = config != null && !string.IsNullOrWhiteSpace(config.SonarrApiUrl) && !string.IsNullOrWhiteSpace(config.SonarrApiKey),
                    AutoSyncEnabled = config?.AutoSync ?? false,
                    SyncIntervalHours = config?.SyncIntervalHours ?? 24,
                    SonarrApiUrl = config?.SonarrApiUrl ?? "",
                    Version = Plugin.Instance?.Version?.ToString() ?? "Unknown"
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error getting plugin status", ex);
                return new { IsConfigured = false, Error = ex.Message };
            }
        }
    }
}