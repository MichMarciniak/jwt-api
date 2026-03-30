using System.Security.Cryptography;
using System.Text;
using JwtApi.Settings;
using JwtApi.Common;
using JwtApi.Data;
using JwtApi.DTOs;
using JwtApi.Entities;
using JwtApi.Services.PasswordHasher;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace JwtApi.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    private readonly IPasswordHasher _hasher;


    public UserService(AppDbContext context,  TokenService tokenService, IPasswordHasher hasher)
    {
        _context = context;
        _tokenService = tokenService;
        _hasher = hasher;
    }

    public async Task<Result<bool>> RegisterAsync(string username, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Name == username))
        {
            return Result<bool>.Failure(Error.UserAlreadyExists);
        }

        var passwordHash = _hasher.Hash(password);
        // no validation for now for ease

        var user = new User
        {
            Name = username,
            Password = passwordHash,
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<AuthDto.AuthResponse>> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
        if (user == null || !_hasher.Verify(password, user.Password))
        {
            return Result<AuthDto.AuthResponse>.Failure(Error.InvalidCredentials);
        }
        var response = new AuthDto.AuthResponse(user.Id, user.Name, user.TokenVersion);
        
        return Result<AuthDto.AuthResponse>.Success(response);
    }

    public async Task<Result<bool>> FullLogout()
    {
        return Result<bool>.Success(true);
    }
}