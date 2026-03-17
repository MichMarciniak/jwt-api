using System.Security.Cryptography;
using System.Text;
using FileUploader.Settings;
using Isopoh.Cryptography.Argon2;

namespace FileUploader.Services.PasswordHasher;

public class IsopohPasswordHasher : IPasswordHasher
{
    private readonly Argon2Settings _settings;

    public IsopohPasswordHasher(Argon2Settings settings)
    {
        _settings = settings;
    }
    
    public string Hash(string password)
    {
        var salt = new byte[_settings.SaltLength];
        RandomNumberGenerator.Fill(salt);

        var config = new Argon2Config
        {
            Password = Encoding.UTF8.GetBytes(password),
            Salt = salt,
            TimeCost = _settings.Iterations,
            MemoryCost = _settings.MemorySize,
            Lanes = _settings.DegreeOfParallelism,
        };
        var hash = Argon2.Hash(config);
        return hash;
    }

    public bool Verify(string password, string hashed)
    {
        var ok = Argon2.Verify(hashed, password);
        return ok;
    }
}