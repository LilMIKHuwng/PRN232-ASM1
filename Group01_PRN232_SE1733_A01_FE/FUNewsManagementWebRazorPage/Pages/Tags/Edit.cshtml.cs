using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static FUNewsManagementWebRazorPage.Pages.SystemAccounts.IndexModel;

namespace FUNewsManagementWebRazorPage.Pages.Tags
{
    [Authorize(Roles = "0,1")]
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public TagUpdateDto Tag { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<TagDto>>($"https://localhost:7015/api/Tags/{id}");
            if (response == null || !response.Success || response.Data == null)
                return NotFound();

            Tag = new TagUpdateDto
            {
                TagId = response.Data.TagId,
                TagName = response.Data.TagName,
                Note = response.Data.Note
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _httpClient.PutAsJsonAsync("https://localhost:7015/api/Tags", Tag);
            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Failed to update tag.");
            return Page();
        }
    }
}
