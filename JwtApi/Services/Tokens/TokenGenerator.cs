using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtApi.Entities;
using JwtApi.DTOs;
using JwtApi.Settings;
using Microsoft.IdentityModel.Tokens;

namespace JwtApi.Services.Tokens;

public class TokenGenerator
{
    private readonly JwtSettings _settings;

    public TokenGenerator(JwtSettings settings)
    {
        _settings = settings;
    }
    
    public AuthDto.Tokens GenerateTokens(AuthDto.AuthResponse authUser)
    {
        var newAccess = GenerateAccessToken(authUser);
        var newRefresh = GenerateRefreshToken(authUser);
        var tokens = new AuthDto.Tokens(newAccess, newRefresh);

        return tokens;
    }
    
    public string GenerateAccessToken(AuthDto.AuthResponse user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim("v", user.Version.ToString()), // version to also revoke access token
            new Claim("typ", "access")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(AuthDto.AuthResponse user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim("v", user.Version.ToString()),
            new Claim("typ", "refresh")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_settings.RefreshDays),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}