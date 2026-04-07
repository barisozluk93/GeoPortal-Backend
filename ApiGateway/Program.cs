using Microsoft.AspNetCore.HttpOverrides;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", false, true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
         options => options.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials());

//app.UseHttpsRedirection();

app.UseOcelot().Wait();

app.UseAuthorization();

app.MapControllers();

app.Run();

//using Microsoft.AspNetCore.HttpOverrides;
//using Ocelot.DependencyInjection;
//using Ocelot.Middleware;

//var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//builder.Services.AddOcelot();
//builder.Services.AddCors();

//builder.Services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.ForwardedHeaders =
//        ForwardedHeaders.XForwardedFor |
//        ForwardedHeaders.XForwardedProto |
//        ForwardedHeaders.XForwardedHost;
//});

//var app = builder.Build();

//app.UseForwardedHeaders();

//app.UsePathBase("/geoPortalApi");

//app.UseCors(options =>
//    options.WithOrigins("http://localhost:4200", "http://178.251.42.243:8080")
//           .AllowAnyHeader()
//           .AllowAnyMethod()
//           .AllowCredentials()
//);

//await app.UseOcelot();

//app.Run();
