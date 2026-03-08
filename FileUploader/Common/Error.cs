namespace FileUploader.Common;

public record Error(string Code, string Message, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, 200);
    
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid credentials", 401);
    
    public static readonly Error UserNotFound = 
        new("Auth.UserNotFound", "User not found", 404);
    
    public static readonly Error UserAlreadyExists = 
        new("Auth.UserAlreadyExists", "User already exists", 409);
    
    
}