using AuditLogManagement.Authorization;
using AuditLogManagement.DbContexts;
using AuditLogManagement.Entities;
using AuditLogManagement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuditLogManagement.Controllers;

[ApiController]
[Route("geoPortalApi/AuditLog")]
[Authorize]
public sealed class AuditLogsController(AuditLogDbContext db) : ControllerBase
{
    [HttpGet("Paginate")]
    [HasPermission("LogScene.Paging.Permission")]
    public async Task<IActionResult> GetAll(
        [FromQuery] PagingParameter pagingParameter,
        [FromQuery] AuditLogQuery request,
        CancellationToken cancellationToken)
    {
        var result = new Result<PagingResult<PagedList<AuditLogRecord>>>();

        await using var transaction =
            await db.Database.BeginTransactionAsync(
                IsolationLevel.RepeatableRead,
                cancellationToken);

        try
        {
            request.PageNumber = Math.Max(1, request.PageNumber);
            request.PageSize = Math.Clamp(request.PageSize, 1, 50);

            var query = db.AuditLogs
                .AsNoTracking()
                .AsQueryable();

            if (request.StartUtc.HasValue)
            {
                query = query.Where(x =>
                    x.TimestampUtc >= request.StartUtc.Value);
            }

            if (request.EndUtc.HasValue)
            {
                query = query.Where(x =>
                    x.TimestampUtc <= request.EndUtc.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                query = query.Where(x =>
                    x.UserId == request.UserId);
            }

            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                query = query.Where(x =>
                    x.UserName != null &&
                    EF.Functions.ILike(
                        x.UserName,
                        $"%{request.UserName}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.ServiceName))
            {
                query = query.Where(x =>
                    x.ServiceName == request.ServiceName);
            }

            if (!string.IsNullOrWhiteSpace(request.ActionType))
            {
                query = query.Where(x =>
                    x.ActionType == request.ActionType);
            }

            if (request.IsSuccess.HasValue)
            {
                query = query.Where(x =>
                    x.IsSuccess == request.IsSuccess.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.CorrelationId))
            {
                query = query.Where(x =>
                    x.CorrelationId == request.CorrelationId);
            }

            query = query.OrderByDescending(x => x.TimestampUtc);

            var pagedList = PagedList<AuditLogRecord>.ToPagedList(
                query,
                request.PageNumber,
                request.PageSize);

            var pagingResult = new PagingResult<PagedList<AuditLogRecord>>
            {
                Items = pagedList,
                TotalCount = pagedList.TotalCount
            };

            result.SetData(pagingResult);
            result.SetIsSuccess(true);
            result.SetMessage("Kayýtlar baþarýyla getirildi.");

            await transaction.CommitAsync(cancellationToken);

            return Ok(result);
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            result.SetData(null!);
            result.SetIsSuccess(false);
            result.SetMessage(exception.Message);

            return BadRequest(result);
        }
    }
}