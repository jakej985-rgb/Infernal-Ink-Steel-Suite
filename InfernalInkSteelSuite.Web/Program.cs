using InfernalInkSteelSuite.Web.Services;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

// Sessions to track logged-in user
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
});

// HttpClient for API
builder.Services.AddHttpClient<ApiClient>((sp, http) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["ApiBaseUrl"] ?? "http://localhost:5000";

    http.BaseAddress = new Uri(baseUrl);
});

// Database & Repositories
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback or throw
    connectionString = "Data Source=shop_manager.db";
}

// Ensure database is initialized (optional for web app if desktop app does it, but good for safety)
// var dbManager = new DatabaseManager(connectionString);
// dbManager.InitializeDatabase();

builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(connectionString));
builder.Services.AddScoped<IClientRepository>(sp => new ClientRepository(connectionString));
builder.Services.AddScoped<IAppointmentRepository>(sp => new AppointmentRepository(connectionString));
builder.Services.AddScoped<IDocumentRepository>(sp => new DocumentRepository(connectionString));
builder.Services.AddScoped<IShopSettingsRepository>(sp => new ShopSettingsRepository(connectionString));

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

app.MapRazorPages();

app.Run();
