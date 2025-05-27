using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static FUNewsManagementWebRazorPage.Pages.SystemAccounts.IndexModel;

namespace FUNewsManagementWebRazorPage.Pages.SystemAccounts
{
    [Authorize(Roles = "0")]
    public class DetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        public SystemAccount Account { get; set; }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<SystemAccount>>($"https://localhost:7015/api/SystemAccounts/{id}");
            if (response?.Success != true)
                return NotFound();

            Account = response.Data;
            return Page();
        }
    }
}
