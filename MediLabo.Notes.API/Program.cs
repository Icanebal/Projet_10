using System.Text;
using MediLabo.Common.HttpServices;
using MediLabo.Notes.API.Data;
using MediLabo.Notes.API.Interfaces;
using MediLabo.Notes.API.Repositories;
using MediLabo.Notes.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Gateway"] ?? "https://localhost:5000/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Service dédié pour les appels à l'API Patients
builder.Services.AddScoped<IPatientService, PatientService>();

var mongoConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value!;
var mongoDatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value!;

builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, mongoDatabaseName));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;
var issuer = jwtSettings["Issuer"]!;
var audience = jwtSettings["Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<NoteService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
