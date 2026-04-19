using InfernalInkSteelSuite.Web.Services;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(); // Add MVC support

builder.Services.AddHttpContextAccessor();

// Sessions to track logged-in user
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
});

// Configure ApiOptions
builder.Services.Configure<ApiOptions>(builder.Configuration);

// HttpClient for API
builder.Services.AddHttpClient<ApiClient>();

// Database & Repositories
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Ensure database directory exists
var dbPath = connectionString.Replace("Data Source=", "");
var dbDir = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
}

// Ensure database is initialized
// var dbManager = new DatabaseManager(connectionString);
// dbManager.InitializeDatabase();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IShopSettingsRepository, ShopSettingsRepository>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// 👇 add this line BEFORE MapRazorPages
app.MapGet("/", () => Results.Redirect("/Account/Login"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();

app.Run();
