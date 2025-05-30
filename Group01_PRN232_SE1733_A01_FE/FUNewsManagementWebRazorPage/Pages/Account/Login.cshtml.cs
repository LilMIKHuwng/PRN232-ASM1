using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FUNewsManagementWebRazorPage.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginRequest LoginRequest { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsJsonAsync("https://localhost:7015/api/SystemAccounts/login", new
                {
                    Email = LoginRequest.UserName,
                    Password = LoginRequest.Password
                });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return Page();
                }

                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(content);

                if (loginResponse != null && loginResponse.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginResponse.Email),
                        new Claim(ClaimTypes.Role, loginResponse.Role.ToString())
                    };

					if (loginResponse.AccountId.HasValue)
					{
						claims.Add(new Claim("AccountId", loginResponse.AccountId.Value.ToString()));
						Response.Cookies.Append("AccountId", loginResponse.AccountId.Value.ToString());
					}

					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    Response.Cookies.Append("UserName", loginResponse.Email);
                    Response.Cookies.Append("Role", loginResponse.Role.ToString());

                    return RedirectToPage("/Index");
                }
            }
            catch (Exception)
            {
                // Log exception if needed
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ModelState.AddModelError("", "Login failure");
            return Page();
        }
    }

    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Role { get; set; }
        public string Email { get; set; }
		public short? AccountId { get; set; }
	}
}
