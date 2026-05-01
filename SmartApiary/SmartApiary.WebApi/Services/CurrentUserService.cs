using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Security;

namespace SmartApiary.WebApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userIdStr, out var userId) ? userId : null;
            }
        }
    }
}
