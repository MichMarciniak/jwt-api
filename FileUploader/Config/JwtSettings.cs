namespace FileUploader.Settings;

public class JwtSettings
{
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Key { get; set; } = "";
    public int AccessMinutes { get; set; } = 60;
    public int RefreshDays { get; set; } = 7;
}