using InfernalInkSteelSuite.Web.Services;

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
