using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using EmbyTags.Models;

namespace EmbyTags.Services
{
    public interface ISonarrService
    {
        Task<List<SonarrSeries>> GetAllSeriesAsync();
        Task<List<SonarrTag>> GetAllTagsAsync();
        Task<bool> TestConnectionAsync();
    }

    public class SimpleSonarrService : ISonarrService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;

        public SimpleSonarrService(ILogger logger, IJsonSerializer jsonSerializer)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _jsonSerializer = jsonSerializer;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var config = Plugin.Instance?.Configuration;
                if (config == null || string.IsNullOrWhiteSpace(config.SonarrApiUrl) || string.IsNullOrWhiteSpace(config.SonarrApiKey))
                {
                    _logger.Error("Sonarr configuration is incomplete");
                    return false;
                }

                // Remove trailing /api/v3 if present, we'll add it ourselves
                var baseUrl = config.SonarrApiUrl.TrimEnd('/');
                _logger.Info($"[DEBUG] Original URL: '{config.SonarrApiUrl}'");
                _logger.Info($"[DEBUG] Base URL after trim: '{baseUrl}'");
                
                if (baseUrl.EndsWith("/api/v3", StringComparison.OrdinalIgnoreCase))
                {
                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 7);
                    _logger.Info($"[DEBUG] Stripped /api/v3 suffix, new base: '{baseUrl}'");
                }

                var finalUrl = $"{baseUrl}/api/v3/system/status";
                _logger.Info($"[DEBUG] Final URL to test: '{finalUrl}'");
                _logger.Info($"[DEBUG] API Key (first 10 chars): '{config.SonarrApiKey.Substring(0, Math.Min(10, config.SonarrApiKey.Length))}...'");

                var request = new HttpRequestMessage(HttpMethod.Get, finalUrl);
                request.Headers.Add("X-Api-Key", config.SonarrApiKey);

                _logger.Info($"Testing Sonarr connection to: {finalUrl}");

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                
                _logger.Info($"[DEBUG] Response StatusCode: {response.StatusCode}");
                _logger.Info($"[DEBUG] Response Body: {responseBody}");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.Info($"Successfully connected to Sonarr API. Response: {responseBody}");
                    return true;
                }
                else
                {
                    _logger.Error($"Failed to connect to Sonarr API: {response.StatusCode} - {response.ReasonPhrase}. Body: {responseBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error testing Sonarr connection", ex);
                return false;
            }
        }

        public async Task<List<SonarrSeries>> GetAllSeriesAsync()
        {
            try
            {
                var config = Plugin.Instance?.Configuration;
                if (config == null || string.IsNullOrWhiteSpace(config.SonarrApiUrl) || string.IsNullOrWhiteSpace(config.SonarrApiKey))
                {
                    _logger.Error("Sonarr configuration is incomplete");
                    return new List<SonarrSeries>();
                }

                // Remove trailing /api/v3 if present, we'll add it ourselves
                var baseUrl = config.SonarrApiUrl.TrimEnd('/');
                if (baseUrl.EndsWith("/api/v3", StringComparison.OrdinalIgnoreCase))
                {
                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 7);
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/v3/series");
                request.Headers.Add("X-Api-Key", config.SonarrApiKey);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var series = _jsonSerializer.DeserializeFromString<List<SonarrSeries>>(content);
                    _logger.Info($"Retrieved {series.Count} series from Sonarr");
                    return series;
                }
                else
                {
                    _logger.Error($"Failed to get series from Sonarr: {response.StatusCode}");
                    return new List<SonarrSeries>();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error getting series from Sonarr", ex);
                return new List<SonarrSeries>();
            }
        }

        public async Task<List<SonarrTag>> GetAllTagsAsync()
        {
            try
            {
                var config = Plugin.Instance?.Configuration;
                if (config == null || string.IsNullOrWhiteSpace(config.SonarrApiUrl) || string.IsNullOrWhiteSpace(config.SonarrApiKey))
                {
                    _logger.Error("Sonarr configuration is incomplete");
                    return new List<SonarrTag>();
                }

                // Remove trailing /api/v3 if present, we'll add it ourselves
                var baseUrl = config.SonarrApiUrl.TrimEnd('/');
                if (baseUrl.EndsWith("/api/v3", StringComparison.OrdinalIgnoreCase))
                {
                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 7);
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/v3/tag");
                request.Headers.Add("X-Api-Key", config.SonarrApiKey);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tags = _jsonSerializer.DeserializeFromString<List<SonarrTag>>(content);
                    _logger.Info($"Retrieved {tags.Count} tags from Sonarr");
                    return tags;
                }
                else
                {
                    _logger.Error($"Failed to get tags from Sonarr: {response.StatusCode}");
                    return new List<SonarrTag>();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error getting tags from Sonarr", ex);
                return new List<SonarrTag>();
            }
        }
    }
}