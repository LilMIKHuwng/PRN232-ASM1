using Microsoft.Extensions.Configuration;
using Repositories.Models;
using Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SystemAccountService
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public SystemAccountService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<List<SystemAccount>> GetAllAsync(string? search)
        {
            var all = await _unitOfWork.SystemAccounts.GetAllAsync();
            return all
                .Where(acc =>
                    string.IsNullOrWhiteSpace(search) ||
                    acc.AccountName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    acc.AccountEmail.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }


        public async Task<SystemAccount?> GetByIdAsync(short id)
            => await _unitOfWork.SystemAccounts.GetByIdAsync(id);

        public async Task<SystemAccount> AddAsync(SystemAccountCreateDto accountDto)
        {
            // Map DTO sang Entity
            var account = new SystemAccount
            {
                AccountId = accountDto.AccountId,
                AccountName = accountDto.AccountName,
                AccountEmail = accountDto.AccountEmail,
                AccountRole = accountDto.AccountRole,
                AccountPassword = accountDto.AccountPassword,
            };

            return await _unitOfWork.SystemAccounts.AddAsync(account);
        }

        public async Task<bool> UpdateAsync(SystemAccountUpdateDto account)
        {
            var existing = await _unitOfWork.SystemAccounts.GetByIdAsync(account.AccountId);
            if (existing == null) return false;

            existing.AccountName = account.AccountName;
            existing.AccountEmail = account.AccountEmail;
            existing.AccountPassword = account.AccountPassword;
            existing.AccountRole = account.AccountRole;

            await _unitOfWork.SystemAccounts.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(short accountId)
        {
            var existingAccount = await _unitOfWork.SystemAccounts.GetByIdAsync(accountId);
            if (existingAccount == null)
                return false;

            await _unitOfWork.SystemAccounts.DeleteAsync(existingAccount);
            return true;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            // Check against Admin from appsettings
            var adminEmail = _configuration["DefaultAdmin:Email"];
            var adminPassword = _configuration["DefaultAdmin:Password"];

            if (loginRequest.Email == adminEmail && loginRequest.Password == adminPassword)
            {
                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Admin login successful.",
                    Role = 0,
                    Email = adminEmail
                };
            }

            // Check database for staff/lecturer
            var accounts = await _unitOfWork.SystemAccounts.GetAllAsync();
            var matched = accounts.FirstOrDefault(acc =>
                acc.AccountEmail == loginRequest.Email &&
                acc.AccountPassword == loginRequest.Password &&
                (acc.AccountRole == 1 || acc.AccountRole == 2));

            if (matched == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password, or role not allowed."
                };
            }

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Role = matched.AccountRole,
                Email = matched.AccountEmail
            };
        }
    }
}
