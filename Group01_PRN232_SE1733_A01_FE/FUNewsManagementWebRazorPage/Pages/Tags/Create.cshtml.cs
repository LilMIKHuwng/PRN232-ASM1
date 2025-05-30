using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.Tags
{
    [Authorize(Roles = "0,1")]
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public TagCreateDto Tag { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7015/api/Tags", Tag);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Failed to create tag.");
            return Page();
        }
    }
}
