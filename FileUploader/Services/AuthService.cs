using System.Security.Cryptography;
using System.Text;
using FileUploader.Data;
using Konscious.Security.Cryptography;

namespace FileUploader.Services;

public class AuthService
{
    private const int Degree = 8;
    private const int Iterations = 4;
    private const int MemorySize = 65536;
    
    // could've used sth higher-level :/ 
    public string HashPassword(string password)
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

    public bool VerifyPassword(string password, string fullHash)
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