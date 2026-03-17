using System.Text;
using FileUploader.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FileUploader.Config;

public class JwtConfig
{
    public static TokenValidationParameters GetValidationParameters(JwtSettings _settings)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience, 
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key))
        };
    }

    public static JwtBearerEvents GetJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access-token"];
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var principal = context.Principal;
                var typeClaim = principal?.FindFirst("typ")?.Value;

                if (typeClaim != "access")
                {
                    context.Fail("Invalid token type. Only access tokens are allowed here");
                }

                return Task.CompletedTask;
            }
        };
    }
}