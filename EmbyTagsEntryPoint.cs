using System;
using System.Threading;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Logging;

namespace EmbyTags
{
    public class EmbyTagsEntryPoint : IServerEntryPoint
    {
        private readonly ILogger _logger;
        private Timer _syncTimer;

        public EmbyTagsEntryPoint(ILogger logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.Info("EmbyTags plugin started");
            
            // Initialize plugin - configuration will be available through Plugin.Instance.Configuration
            SetupSyncTimer();
        }

        private void SetupSyncTimer()
        {
            try
            {
                _syncTimer?.Dispose();
                _syncTimer = null;

                var config = Plugin.Instance?.Configuration;
                if (config == null)
                {
                    _logger.Info("Plugin configuration not available yet");
                    return;
                }
                
                if (!config.AutoSync || string.IsNullOrWhiteSpace(config.SonarrApiUrl) || string.IsNullOrWhiteSpace(config.SonarrApiKey))
                {
                    _logger.Info("Automatic sync is disabled or configuration is incomplete");
                    return;
                }

                var intervalMs = config.SyncIntervalHours * 60 * 60 * 1000;
                _logger.Info($"Setting up automatic sync timer with interval: {config.SyncIntervalHours} hours");

                _syncTimer = new Timer(OnSyncTimerElapsed, null, TimeSpan.FromMilliseconds(intervalMs), TimeSpan.FromMilliseconds(intervalMs));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error setting up sync timer", ex);
            }
        }

        private void OnSyncTimerElapsed(object state)
        {
            try
            {
                _logger.Info("Automatic sync timer elapsed - sync functionality will be implemented via API");
                // The actual sync will be triggered via the REST API endpoints
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error during automatic sync", ex);
            }
        }

        public void Dispose()
        {
            try
            {
                _syncTimer?.Dispose();
                _syncTimer = null;
                _logger.Info("EmbyTags plugin stopped");
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error disposing EmbyTags plugin", ex);
            }
        }
    }
}