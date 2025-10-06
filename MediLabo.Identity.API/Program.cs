using MediLabo.Identity.API.Data;
using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;              
    options.Password.RequireLowercase = true;          
    options.Password.RequireUppercase = true;          
    options.Password.RequireNonAlphanumeric = true;    
    options.Password.RequiredLength = 6;               

    options.User.RequireUniqueEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
    options.Lockout.MaxFailedAccessAttempts = 5;                      
    options.Lockout.AllowedForNewUsers = true;                        
})
.AddEntityFrameworkStores<IdentityDbContext>()
.AddDefaultTokenProviders();

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

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<IdentityDbContext>();

        context.Database.Migrate();

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialized and migrations applied successfully");

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await RoleInitializer.InitializeAsync(roleManager, userManager);
        logger.LogInformation("Roles and admin user initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
