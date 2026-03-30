using System.Security.Cryptography;
using System.Text;
using JwtApi.Settings;
using Konscious.Security.Cryptography;

namespace JwtApi.Services.PasswordHasher;

public class ManualPasswordHasher : IPasswordHasher
{
    private readonly Argon2Settings _settings;
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public ManualPasswordHasher(Argon2Settings settings)
    {
        _settings = settings;
    }
    
    public bool Verify(string password, string hashed)
    {
        var parts = hashed.Split(':');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        { 
            Salt = salt,
            DegreeOfParallelism = _settings.DegreeOfParallelism,
            Iterations = _settings.Iterations,
            MemorySize = _settings.MemorySize 
        };

        var newHash = argon2.GetBytes(_settings.HashLength);
        
        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }


    public string Hash(string password)
    {
        var salt = new byte[_settings.SaltLength];
        RandomNumberGenerator.Fill(salt);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = _settings.DegreeOfParallelism,
            Iterations = _settings.Iterations,
            MemorySize = _settings.MemorySize 
        };
        var hash = argon2.GetBytes(_settings.HashLength);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

}