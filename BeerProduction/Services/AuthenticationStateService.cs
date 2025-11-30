using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeerProduction.Services
{
    public class AuthenticationStateService : AuthenticationStateProvider
    {
        private static ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public Task LoginAsync(string userId, string username, string roleName)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, roleName)
            }, "Authentication");

            _currentUser = new ClaimsPrincipal(identity);
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            
            return Task.CompletedTask;
        }

        public Task LogoutAsync()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            
            return Task.CompletedTask;
        }

        public string? GetUserId()
        {
            return _currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string? GetUsername()
        {
            return _currentUser.Identity?.Name;
        }

        public string? GetRole()
        {
            return _currentUser.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
