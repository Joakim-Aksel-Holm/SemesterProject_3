using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeerProduction.Services
{
    public class AuthenticationStateService : AuthenticationStateProvider
    {
        /// <summary>
        /// Constructs the current user (static)
        /// </summary>
        private static ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        // =========================================================================
        // Authentication methods (async)
        // =========================================================================
        
        /// <summary>
        /// Gets the current user authentication state
        /// </summary>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }
        
        /// <summary>
        /// Inputs the user credentials and claims an identity
        /// </summary>
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
        
        /// <summary>
        /// Returns the current user to an unauthenticated state by nullifying the identity
        /// </summary>
        public Task LogoutAsync()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            
            Console.WriteLine("Successfully logged out");
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the current user's id through the claim 'NameIdentifier'
        /// </summary>
        public string? GetUserId()
        {
            return _currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Gets the current user's username through the claim 'Identity'
        /// </summary>
        public string? GetUsername()
        {
            return _currentUser.Identity?.Name;
        }

        /// <summary>
        /// Gets the current user's role through the claim 'Role'
        /// </summary>
        public string? GetRole()
        {
            return _currentUser.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
