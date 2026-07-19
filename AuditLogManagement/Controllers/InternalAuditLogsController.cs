using AuditLog.Shared;
using AuditLogManagement.DbContexts;
using AuditLogManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuditLogManagement.Controllers;

[ApiController]
public sealed class InternalAuditLogsController(AuditLogDbContext db, IConfiguration configuration) : ControllerBase
{
    [HttpPost("/internal/auditlogs")]
    public async Task<IActionResult> Create([FromBody] AuditLogEntry entry, CancellationToken cancellationToken)
    {
        var expected = configuration["AuditLog:InternalApiKey"];
        var actual = Request.Headers["X-Audit-Api-Key"].ToString();
        if (string.IsNullOrWhiteSpace(expected) || !string.Equals(expected, actual, StringComparison.Ordinal)) return Unauthorized();

        db.AuditLogs.Add(new AuditLogRecord
        {
            TimestampUtc = entry.TimestampUtc,
            ServiceName = entry.ServiceName,
            UserId = entry.UserId,
            UserName = entry.UserName,
            Roles = entry.Roles,
            ActionType = entry.ActionType,
            HttpMethod = entry.HttpMethod,
            Path = entry.Path,
            QueryString = entry.QueryString,
            StatusCode = entry.StatusCode,
            IsSuccess = entry.IsSuccess,
            DurationMs = entry.DurationMs,
            IpAddress = entry.IpAddress,
            UserAgent = entry.UserAgent,
            CorrelationId = entry.CorrelationId,
            ErrorMessage = entry.ErrorMessage
        });
        await db.SaveChangesAsync(cancellationToken);
        return Accepted();
    }
}
