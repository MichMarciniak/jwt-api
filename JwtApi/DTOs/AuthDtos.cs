using System.ComponentModel.DataAnnotations;

namespace JwtApi.DTOs;


public static class AuthDto
{
    public record Login(string Username, string Password);
    public record Register(
        [Required] [MaxLength(100)] string Username,
        [Required] string Password
    );

    public record Tokens(
        string AccessToken,
        string RefreshToken
    );

    public record AuthResponse(int UserId, string Username, int Version);

}
