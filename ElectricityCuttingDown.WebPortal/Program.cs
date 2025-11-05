using ElectricityCuttingDown.WebPortal.Data;
using ElectricityCuttingDown.WebPortal.Middleware;
using ElectricityCuttingDown.WebPortal.Services;
using ElectricityCuttingDown.WebPortal.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddLogging();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddHttpClient<IApiClient, ApiClient>();
// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Add DbContext >>>main
builder.Services.AddDbContext<Electricity_FTAContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FTA")));

//Add DbContext for STA (Authentication database)
builder.Services.AddDbContext<Electricity_STAContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("STA")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.Run();
