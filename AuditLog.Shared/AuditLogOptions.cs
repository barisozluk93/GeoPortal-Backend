namespace AuditLog.Shared;

public sealed class AuditLogOptions
{
    public string ServiceName { get; set; } = "UnknownService";
    public string BaseUrl { get; set; } = "http://localhost:5050";
    public string IngestionPath { get; set; } = "/internal/auditlogs";
    public string InternalApiKey { get; set; } = "GeoPortalAuditInternalKey-ChangeMe";
    public bool Enabled { get; set; } = true;
    public string[] ExcludedPathPrefixes { get; set; } = ["/swagger", "/health", "/favicon.ico"];
}
