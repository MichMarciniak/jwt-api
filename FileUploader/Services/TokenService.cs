using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FileUploader.Common;
using FileUploader.Config;
using FileUploader.Data;
using FileUploader.DTOs;
using FileUploader.Services.Tokens;
using FileUploader.Settings;

namespace FileUploader.Services;

public class TokenService
{
    private AppDbContext _context;
    private readonly JwtSettings _settings;
    private readonly TokenGenerator _generator;

    public TokenService(JwtSettings settings, AppDbContext context, TokenGenerator generator)
    {
        _settings = settings;
        _context = context;
        _generator = generator;
    }

    public AuthDto.Tokens GenerateTokens(AuthDto.AuthResponse user)
    {
        return _generator.GenerateTokens(user);
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var parameters = JwtConfig.GetValidationParameters(_settings);

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

        var typeClaim = principal.FindFirstValue("typ");
        if (typeClaim != "refresh")
        {
            return Result<AuthDto.Tokens>.Failure(Error.InvalidToken);
        }
        
        var userId = int.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var versionInToken = int.Parse(principal.FindFirstValue("v") ?? "0");
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.TokenVersion != versionInToken)
        {
            return Result<AuthDto.Tokens>.Failure(Error.InvalidToken);
        }

        var authUser = new AuthDto.AuthResponse(user.Id, user.Name, user.TokenVersion);

        var tokens = _generator.GenerateTokens(authUser);

        return Result<AuthDto.Tokens>.Success(tokens);
    }

}



