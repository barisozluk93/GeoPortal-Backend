namespace History.Shared;
public sealed class HistoryOptions
{
    public bool Enabled { get; set; } = true;
    public string BaseUrl { get; set; } = "http://localhost:5051";
    public string IngestionPath { get; set; } = "/internal/histories";
    public string InternalApiKey { get; set; } = "GeoPortalHistoryInternalKey-ChangeMe";
    public string ServiceName { get; set; } = string.Empty;
}
