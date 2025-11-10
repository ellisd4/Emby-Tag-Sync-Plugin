using System.Collections.Generic;

namespace EmbyTags.Models
{
    public class SonarrSeries
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleSlug { get; set; }
        public int TvdbId { get; set; }
        public int TmdbId { get; set; }
        public string ImdbId { get; set; }
        public int Year { get; set; }
        public string Path { get; set; }
        public int QualityProfileId { get; set; }
        public bool Monitored { get; set; }
        public string Status { get; set; }
        public List<int> Tags { get; set; } = new List<int>();
        public SonarrImage[] Images { get; set; }
        public int Runtime { get; set; }
        public string Network { get; set; }
        public string AirTime { get; set; }
        public string Overview { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
    }

    public class SonarrImage
    {
        public string CoverType { get; set; }
        public string Url { get; set; }
        public string RemoteUrl { get; set; }
    }

    public class SonarrTag
    {
        public int Id { get; set; }
        public string Label { get; set; }
    }

    public class SonarrSystemStatus
    {
        public string Version { get; set; }
        public string Branch { get; set; }
        public string BuildTime { get; set; }
        public bool IsDebug { get; set; }
        public bool IsProduction { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUserInteractive { get; set; }
        public string StartupPath { get; set; }
        public string AppData { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public string RuntimeVersion { get; set; }
        public string RuntimeName { get; set; }
    }

    public class TestConnectionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int SeriesCount { get; set; }
        public string SonarrVersion { get; set; }
    }

    public class SyncResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        public int ProcessedSeries { get; set; }
        public int UpdatedSeries { get; set; }
        public int SkippedSeries { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }

    public class SeriesMatch
    {
        public string EmbyId { get; set; }
        public string EmbyName { get; set; }
        public int SonarrId { get; set; }
        public string SonarrName { get; set; }
        public int? TvdbId { get; set; }
        public int? TmdbId { get; set; }
        public string ImdbId { get; set; }
        public List<string> CurrentTags { get; set; } = new List<string>();
        public List<string> NewTags { get; set; } = new List<string>();
        public bool TagsChanged { get; set; }
    }
}