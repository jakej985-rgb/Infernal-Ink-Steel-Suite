using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Security.Principal;

namespace InfernalInkSteelSuite.Web.Pages
{
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class DebugModel(IConfiguration configuration, IWebHostEnvironment env) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IWebHostEnvironment _env = env;

        public string ContentRootPath { get; set; } = "";
        public string WebRootPath { get; set; } = "";
        public string ProcessName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string ConnectionStringConfig { get; set; } = "";
        public string DbPath { get; set; } = "";
        public bool DirExists { get; set; }
        public bool FileExists { get; set; }
        public string FileAttributes { get; set; } = "";
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public string ConnectionResult { get; set; } = "";
        public string ConnectionError { get; set; } = "";

        public Microsoft.AspNetCore.Mvc.IActionResult OnGet()
        {
            if (!_env.IsDevelopment())
            {
                return NotFound();
            }

            ContentRootPath = _env.ContentRootPath;
            WebRootPath = _env.WebRootPath;
            ProcessName = Process.GetCurrentProcess().ProcessName;
            if (OperatingSystem.IsWindows())
            {
                UserName = WindowsIdentity.GetCurrent().Name;
            }
            else
            {
                UserName = "Non-Windows Environment";
            }

            ConnectionStringConfig = "Direct database access disabled for Web Portal (H5).";
            ConnectionResult = "N/A - Use API Health endpoints instead.";

            return Page();
        }
    }
}
