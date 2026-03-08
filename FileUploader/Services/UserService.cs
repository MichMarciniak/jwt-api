using FileUploader.Common;
using FileUploader.Data;
using FileUploader.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileUploader.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    
    public UserService(AppDbContext context,  AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Name == username))
        {
            return false;
        }

        var passwordHash = _authService.HashPassword(password);

        var user = new User
        {
            Name = username,
            Password = passwordHash,
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Result<User>> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
        if (user == null || !_authService.VerifyPassword(password, user.Password))
        {
            return Result<User>.Failure(Error.InvalidCredentials);
        }
        
        return Result<User>.Success(user);
    }
    
}