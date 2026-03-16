using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FileUploader.Config;

public class JwtConfig
{
    public static TokenValidationParameters GetValidationParameters(IConfiguration config)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JWT_ISSUER"],
            ValidAudience = config["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["JWT_KEY"]))
        };
    }
}