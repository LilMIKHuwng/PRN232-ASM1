using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static FUNewsManagementWebRazorPage.Pages.Categories.CreateModel;

namespace FUNewsManagementWebRazorPage.Pages.Account
{
    public class UpdateProfileModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UpdateProfileModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public SystemAccount Account { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var accountIdClaim = User.FindFirst("AccountId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim)) return RedirectToPage("/Account/Login");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetFromJsonAsync<ApiResponse<SystemAccount>>(
                $"https://localhost:7015/api/SystemAccounts/{accountIdClaim}");

            if (response == null || !response.Success)
            {
                ModelState.AddModelError("", "Cannot retrieve account.");
                return Page();
            }

            Account = response.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var result = await client.PutAsJsonAsync($"https://localhost:7015/api/SystemAccounts", Account);

            if (!result.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Update failed.");
                return Page();
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToPage("/Index");
        }
    }
}
