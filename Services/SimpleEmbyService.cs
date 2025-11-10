using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Logging;

namespace EmbyTags.Services
{
    public interface IEmbyService
    {
        List<Series> GetAllTvSeries();
        bool AddTagToSeries(Guid seriesId, string tag);
        bool RemoveTagFromSeries(Guid seriesId, string tag);
        Series FindSeriesByExternalId(string tvdbId, string tmdbId);
    }

    public class SimpleEmbyService : IEmbyService
    {
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger _logger;

        public SimpleEmbyService(ILibraryManager libraryManager, ILogger logger)
        {
            _libraryManager = libraryManager;
            _logger = logger;
        }

        public List<Series> GetAllTvSeries()
        {
            try
            {
                var allItems = _libraryManager.GetItemList(new MediaBrowser.Controller.Entities.InternalItemsQuery
                {
                    IncludeItemTypes = new[] { "Series" },
                    Recursive = true
                });

                var series = allItems.OfType<Series>().ToList();
                _logger.Info($"Found {series.Count} TV series in Emby library");
                return series;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error getting TV series from Emby", ex);
                return new List<Series>();
            }
        }

        public Series FindSeriesByExternalId(string tvdbId, string tmdbId)
        {
            try
            {
                var allSeries = GetAllTvSeries();
                
                foreach (var series in allSeries)
                {
                    // Try TVDB ID first
                    if (!string.IsNullOrEmpty(tvdbId) && series.ProviderIds != null)
                    {
                        if (series.ProviderIds.TryGetValue("Tvdb", out var seriesTvdbId) && 
                            seriesTvdbId.Equals(tvdbId, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.Debug($"Matched series '{series.Name}' by TVDB ID: {tvdbId}");
                            return series;
                        }
                    }
                    
                    // Try TMDB ID
                    if (!string.IsNullOrEmpty(tmdbId) && series.ProviderIds != null)
                    {
                        if (series.ProviderIds.TryGetValue("Tmdb", out var seriesTmdbId) && 
                            seriesTmdbId.Equals(tmdbId, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.Debug($"Matched series '{series.Name}' by TMDB ID: {tmdbId}");
                            return series;
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error finding series by external ID", ex);
                return null;
            }
        }

        public bool AddTagToSeries(Guid seriesId, string tag)
        {
            try
            {
                var series = _libraryManager.GetItemById(seriesId) as Series;
                if (series == null)
                {
                    _logger.Error($"Series with ID {seriesId} not found");
                    return false;
                }
                
                if (series.Tags == null)
                {
                    series.Tags = new string[] { };
                }
                
                if (!series.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    var tagList = series.Tags.ToList();
                    tagList.Add(tag);
                    series.Tags = tagList.ToArray();
                    
                    // Save changes to database
                    _libraryManager.UpdateItem(series, series.Parent, MediaBrowser.Controller.Library.ItemUpdateType.MetadataEdit);
                    
                    _logger.Info($"Added tag '{tag}' to series '{series.Name}'");
                    return true;
                }
                else
                {
                    _logger.Debug($"Tag '{tag}' already exists on series '{series.Name}'");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"Error adding tag '{tag}' to series {seriesId}", ex);
                return false;
            }
        }

        public bool RemoveTagFromSeries(Guid seriesId, string tag)
        {
            try
            {
                var series = _libraryManager.GetItemById(seriesId) as Series;
                if (series == null)
                {
                    _logger.Error($"Series with ID {seriesId} not found");
                    return false;
                }
                
                if (series.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    var tagList = series.Tags.ToList();
                    tagList.Remove(tag);
                    series.Tags = tagList.ToArray();
                    
                    // Save changes to database
                    _libraryManager.UpdateItem(series, series.Parent, MediaBrowser.Controller.Library.ItemUpdateType.MetadataEdit);
                    
                    _logger.Info($"Removed tag '{tag}' from series '{series.Name}'");
                    return true;
                }
                else
                {
                    _logger.Debug($"Tag '{tag}' does not exist on series '{series.Name}'");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"Error removing tag '{tag}' from series {seriesId}", ex);
                return false;
            }
        }
    }
}