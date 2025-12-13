using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Services.UserService
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
    _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value
    ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
    ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
    ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    }

}
