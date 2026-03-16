using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FileUploader.Common;
using FileUploader.Config;
using FileUploader.Data;
using FileUploader.DTOs;
using FileUploader.Entities;
using Microsoft.IdentityModel.Tokens;

namespace FileUploader.Services;

public class TokenService
{
    private IConfiguration _config;
    private AppDbContext _context;

    public TokenService(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_KEY"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JWT_ISSUER"],
            audience: _config["JWT_AUDIENCE"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_config["JWT_ACCESS_MINUTES"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim("v", user.TokenVersion.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_KEY"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JWT_ISSUER"],
            audience: _config["JWT_AUDIENCE"],
            claims: claims,
            expires: DateTime.Now.AddDays(int.Parse(_config["JWT_REFRESH_DAYS"]!)),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    //this is just for refresh, normal auth happens automatically
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var parameters = JwtConfig.GetValidationParameters(_config);

        // ignore exp date so i can refresh
        parameters.ValidateLifetime = false;

        try
        {
            return tokenHandler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Result<AuthDto.Tokens>> RefreshToken(string refreshToken)
    {
        var principal = GetPrincipalFromToken(refreshToken);
        if (principal == null)
            return Result<AuthDto.Tokens>.Failure(Error.InvalidToken);
        
        var userId = int.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var versionInToken = int.Parse(principal.FindFirstValue("v") ?? "0");
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.TokenVersion != versionInToken)
        {
            return Result<AuthDto.Tokens>.Failure(Error.InvalidToken);
        }

        var newAccess = GenerateAccessToken(user);
        var newRefresh = GenerateRefreshToken(user);
        var tokens = new AuthDto.Tokens(newAccess, newRefresh);

        return Result<AuthDto.Tokens>.Success(tokens);

    }

}



