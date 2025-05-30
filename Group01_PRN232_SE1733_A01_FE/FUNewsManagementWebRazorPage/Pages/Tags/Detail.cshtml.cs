using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static FUNewsManagementWebRazorPage.Pages.SystemAccounts.IndexModel;

namespace FUNewsManagementWebRazorPage.Pages.Tags
{
    [Authorize(Roles = "0,1")]
    public class DetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public TagDto Tag { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<TagDto>>($"https://localhost:7015/api/Tags/{id}");
            if (response == null || !response.Success || response.Data == null)
                return NotFound();

            Tag = response.Data;
            return Page();
        }
    }
}
