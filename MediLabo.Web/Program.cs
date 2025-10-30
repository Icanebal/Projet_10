using MediLabo.Web.Services;
using MediLabo.Common.HttpServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    var gatewayBaseUrl = builder.Configuration["Gateway:BaseUrl"];
    client.BaseAddress = new Uri(gatewayBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<AssessmentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
