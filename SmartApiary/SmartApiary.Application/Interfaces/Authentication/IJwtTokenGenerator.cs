using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}