using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AuditLogManagement.DbContexts;
#nullable disable
namespace AuditLogManagement.Migrations;
[DbContext(typeof(AuditLogDbContext))]
[Migration("20260718190000_InitialAuditLog")]
public partial class InitialAuditLog : Migration
{
 protected override void Up(MigrationBuilder migrationBuilder)
 {
  migrationBuilder.CreateTable(name:"AuditLogs",columns:table=>new
  {
   Id=table.Column<long>(type:"bigint",nullable:false).Annotation("Npgsql:ValueGenerationStrategy",NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
   TimestampUtc=table.Column<DateTime>(type:"timestamp with time zone",nullable:false), ServiceName=table.Column<string>(type:"character varying(100)",maxLength:100,nullable:false),
   UserId=table.Column<string>(type:"text",nullable:true),UserName=table.Column<string>(type:"text",nullable:true),Roles=table.Column<string>(type:"text",nullable:true),
   ActionType=table.Column<string>(type:"character varying(100)",maxLength:100,nullable:false),HttpMethod=table.Column<string>(type:"character varying(10)",maxLength:10,nullable:false),
   Path=table.Column<string>(type:"character varying(2000)",maxLength:2000,nullable:false),QueryString=table.Column<string>(type:"text",nullable:true),StatusCode=table.Column<int>(type:"integer",nullable:false),
   IsSuccess=table.Column<bool>(type:"boolean",nullable:false),DurationMs=table.Column<long>(type:"bigint",nullable:false),IpAddress=table.Column<string>(type:"text",nullable:true),
   UserAgent=table.Column<string>(type:"text",nullable:true),CorrelationId=table.Column<string>(type:"character varying(200)",maxLength:200,nullable:false),ErrorMessage=table.Column<string>(type:"text",nullable:true)
  },constraints:table=>table.PrimaryKey("PK_AuditLogs",x=>x.Id));
  migrationBuilder.CreateIndex(name:"IX_AuditLogs_ActionType",table:"AuditLogs",column:"ActionType");
  migrationBuilder.CreateIndex(name:"IX_AuditLogs_CorrelationId",table:"AuditLogs",column:"CorrelationId");
  migrationBuilder.CreateIndex(name:"IX_AuditLogs_ServiceName",table:"AuditLogs",column:"ServiceName");
  migrationBuilder.CreateIndex(name:"IX_AuditLogs_TimestampUtc",table:"AuditLogs",column:"TimestampUtc");
  migrationBuilder.CreateIndex(name:"IX_AuditLogs_UserId",table:"AuditLogs",column:"UserId");
 }
 protected override void Down(MigrationBuilder migrationBuilder)=>migrationBuilder.DropTable(name:"AuditLogs");
}
