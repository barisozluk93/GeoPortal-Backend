using System.Text;
using System.Text.Json.Serialization;
using HistoryManagement.DbContexts;
using HistoryManagement.Interfaces;
using HistoryManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HistoryManagementContext>(o => o.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false; o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = configuration["AppSettings:ValidIssuer"], ValidAudience = configuration["AppSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Secret"]!)),
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();
builder.Host.UseWindowsService();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
else { using var scope = app.Services.CreateScope(); scope.ServiceProvider.GetRequiredService<HistoryManagementContext>().Database.Migrate(); }
app.UseHttpsRedirection(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); app.Run();
