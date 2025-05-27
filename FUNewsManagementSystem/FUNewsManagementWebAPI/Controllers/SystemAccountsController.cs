using FUNewsManagementWebAPI.Utils;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.SystemAccountService;

namespace FUNewsManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountService _service;

        public SystemAccountsController(ISystemAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var accounts = await _service.GetAllAsync(search);
            return Ok(new ApiResponse<List<SystemAccount>>(true, "Account list retrieved successfully.", accounts));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var account = await _service.GetByIdAsync(id);
            if (account == null)
                return NotFound(new ApiResponse<SystemAccount>(false, "Account not found."));

            return Ok(new ApiResponse<SystemAccount>(true, "Account retrieved successfully.", account));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SystemAccountCreateDto account)
        {
            var created = await _service.AddAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = created.AccountId },
                new ApiResponse<SystemAccount>(true, "Account created successfully.", created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SystemAccountUpdateDto account)
        {
            var result = await _service.UpdateAsync(account);
            if (result)
                return Ok(new ApiResponse<string>(true, "Account updated successfully."));

            return NotFound(new ApiResponse<string>(false, "Account not found."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _service.DeleteAsync(id);
            if (result)
                return Ok(new ApiResponse<string>(true, "Account deleted successfully."));

            return NotFound(new ApiResponse<string>(false, "Cannot delete category. It may be in use or not found."));
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var result = await _service.LoginAsync(loginRequest);
            return result.Success
                ? Ok(result)
                : Unauthorized(result);
        }

    }

}
