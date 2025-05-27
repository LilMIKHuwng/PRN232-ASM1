using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.SystemAccounts
{
    [Authorize(Roles = "0")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public List<SystemAccount> Accounts { get; set; } = new();

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<SystemAccount>>>(
                "https://localhost:7015/api/SystemAccounts?search=" + Uri.EscapeDataString(Search ?? "")
            );

            if (response != null && response.Success)
            {
                Accounts = response.Data;
            }
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
