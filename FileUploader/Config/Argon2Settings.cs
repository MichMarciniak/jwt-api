namespace FileUploader.Settings;

public class Argon2Settings
{
    public int DegreeOfParallelism { get; set; } = 8;
    public int Iterations { get; set; } = 4;
    public int MemorySize { get; set; } = 65536; //KB
    public int SaltLength { get; set; } = 16;
    public int HashLength { get; set; } = 32;
}