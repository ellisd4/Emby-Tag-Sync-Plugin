using MediaBrowser.Model.Plugins;

namespace EmbyTags
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string SonarrApiUrl { get; set; } = "";
        public string SonarrApiKey { get; set; } = "";
        public bool DebugMode { get; set; } = false;
        public bool DryRun { get; set; } = false;
        public int SyncIntervalHours { get; set; } = 24;
        public bool AutoSync { get; set; } = false;
        public bool OverwriteExistingTags { get; set; } = false;
        public string TagPrefix { get; set; } = "";
    }
}