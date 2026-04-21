using InfernalInkSteelSuite.Web.Services;

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
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Configure ApiOptions
builder.Services.Configure<ApiOptions>(builder.Configuration);

builder.Services.AddTransient<AuthHeaderHandler>();
// HttpClient for API
builder.Services.AddHttpClient<ApiClient>()
    .AddHttpMessageHandler<AuthHeaderHandler>();

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
