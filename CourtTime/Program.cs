using System.Globalization;
using CourtTime.Components;
using CourtTime.Components.Account;
using CourtTime.Data;
using CourtTime.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Consistent English formatting for dates and prices, whatever the server's regional settings are.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

// Blazor Web App. Pages are rendered on the server (static SSR with enhanced forms),
// which keeps cookie authentication simple and works without JavaScript.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Authentication: ASP.NET Core Identity with cookies.
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
builder.Services.AddAuthorization();

// Database: EF Core with SQL Server (LocalDB in development, Azure SQL in production).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Application services.
builder.Services.AddSingleton<IClock, FacilityClock>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<FacilityService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Creates the schema and roles on first run; demo data is added in Development only.
await DbInitializer.InitializeAsync(app.Services, seedDemoData: app.Environment.IsDevelopment());

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAccountEndpoints();

app.Run();
