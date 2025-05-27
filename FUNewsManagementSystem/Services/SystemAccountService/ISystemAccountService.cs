using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SystemAccountService
{
    public interface ISystemAccountService
    {
        Task<List<SystemAccount>> GetAllAsync(string? search);
        Task<SystemAccount?> GetByIdAsync(short id);
        Task<SystemAccount> AddAsync(SystemAccountCreateDto account);
        Task<bool> UpdateAsync(SystemAccountUpdateDto account);
        Task<bool> DeleteAsync(short id);

        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    }

}
