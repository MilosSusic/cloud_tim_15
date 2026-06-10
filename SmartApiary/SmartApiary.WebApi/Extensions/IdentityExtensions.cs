using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SmartApiary.WebApi.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtIssuer = configuration["JwtOptions:Issuer"]
                ?? throw new InvalidOperationException("JwtOptions:Issuer is not configured.");
            var jwtAudience = configuration["JwtOptions:Audience"]
                ?? throw new InvalidOperationException("JwtOptions:Audience is not configured.");
            var jwtKey = configuration["JwtOptions:Key"]
                ?? throw new InvalidOperationException("JwtOptions:Key is not configured.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
