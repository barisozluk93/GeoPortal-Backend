using History.Shared;
using AuditLog.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserManagement.Authorization;
using UserManagement.DbContexts;
using UserManagement.Interfaces;
using UserManagement.Model;
using UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var emailConfig = Configuration
        .GetSection("MailSettings")
        .Get<MailSettings>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddDbContext<UserManagementContext>((sp, options) =>
{
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<HistorySaveChangesInterceptor>());
});

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddHttpClient<IExportGateway, ExportGateway>(client =>
{
    client.BaseAddress = new Uri(Configuration["AppSettings:ApiUrl"]);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.SaveToken = true;
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["AppSettings:ValidIssuer"],
        ValidAudience = builder.Configuration["AppSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGeoPortalAuditLogging(builder.Configuration, "UserManagement");
builder.Host.UseWindowsService();

builder.Services.AddGeoPortalHistory(builder.Configuration, "UserManagement");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
    scope.ServiceProvider.GetRequiredService<UserManagementContext>().Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseGeoPortalAuditLogging();

app.UseAuthorization();

app.MapControllers();

//app.Run("http://159.223.219.238:5222");
app.Run();

