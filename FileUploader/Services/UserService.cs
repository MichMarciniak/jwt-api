using System.Security.Cryptography;
using System.Text;
using FileUploader.Common;
using FileUploader.Data;
using FileUploader.Entities;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace FileUploader.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    
    private const int Degree = 8;
    private const int Iterations = 4;
    private const int MemorySize = 65536;
    private readonly  IConfiguration _config;
    
    public UserService(AppDbContext context,  TokenService tokenService, IConfiguration config)
    {
        _context = context;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<Result<bool>> RegisterAsync(string username, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Name == username))
        {
            return Result<bool>.Failure(Error.UserAlreadyExists);
        }

        var passwordHash = HashPassword(password);
        // later validate password

        var user = new User
        {
            Name = username,
            Password = passwordHash,
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<User>> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
        if (user == null || !VerifyPassword(password, user.Password))
        {
            return Result<User>.Failure(Error.InvalidCredentials);
        }
        
        return Result<User>.Success(user);
    }

    public async Task<Result<bool>> FullLogout()
    {
        return Result<bool>.Success(true);
    }

    
    /* HELPERS ----------------- */ 
    private string HashPassword(string password)
    {
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Degree,
            Iterations = Iterations,
            MemorySize = MemorySize
        };

        var hash = argon2.GetBytes(32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private bool VerifyPassword(string password, string fullHash)
    {
        var parts = fullHash.Split(':');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Degree,
            Iterations = Iterations,
            MemorySize = MemorySize
        };

        var newHash = argon2.GetBytes(32);
        
        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }
}