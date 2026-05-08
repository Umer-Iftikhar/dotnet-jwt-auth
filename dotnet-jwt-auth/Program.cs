using Microsoft.EntityFrameworkCore;
using dotnet_jwt_auth.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

builder.Services.AddDbContext<dotnet_jwt_auth.Data.AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>
(
).AddEntityFrameworkStores<dotnet_jwt_auth.Data.AppDbContext>()
.AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication(); 

app.UseAuthorization();

app.MapControllers();

app.Run();
