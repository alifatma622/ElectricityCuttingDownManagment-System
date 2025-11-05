using System;
using Microsoft.EntityFrameworkCore;
using ElectricityCuttingDown.WebPortal.Data;
using ElectricityCuttingDown.WebPortal.Models.Entities;

namespace ElectricityCuttingDown.WebPortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly Electricity_STAContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(Electricity_STAContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _context.dbo_Users
                    .FirstOrDefaultAsync(u => u.Name == username);

                if (user == null)
                {
                    _logger.LogWarning($"User not found: {username}");

                    // Allow demo fallback for local development: admin/admin
                    if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase) && password == "admin")
                    {
                        _logger.LogInformation("Demo admin authenticated using fallback credentials.");
                        return new User { User_Key = -1, Name = "admin", Password = "admin" };
                    }

                    return null;
                }

                if (user.Password == password)
                {
                    _logger.LogInformation($"User authenticated: {username}");
                    return user;
                }

                // Also allow demo fallback when DB user exists but developer wants to use admin/admin
                if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase) && password == "admin")
                {
                    _logger.LogInformation("Demo admin authenticated using fallback credentials.");
                    return new User { User_Key = -1, Name = "admin", Password = "admin" };
                }

                _logger.LogWarning($"Invalid password for user: {username}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Authentication error: {ex.Message}");
                return null;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.dbo_Users
                    .FirstOrDefaultAsync(u => u.Name == username);

                if (user == null && string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    // return demo admin when requested
                    return new User { User_Key = -1, Name = "admin", Password = "admin" };
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user: {ex.Message}");
                return null;
            }
        }
    }
}
