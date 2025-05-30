using FUNewsManagementWebRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementWebRazorPage.Pages.Tags
{
    [Authorize(Roles = "0,1")]
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(IHttpClientFactory factory)
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7015/api/Tags/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Failed to delete tag.");
            return Page();
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
