using Microsoft.AspNetCore.Identity;

namespace MiniGram.Api.Common;

public interface IAuthService
{
    string HashPassword(string password);
    bool CheckPassword(string? password, string hashedPassword);
}

public class AuthService(IPasswordHasher<object> hasher) : IAuthService
{
    public string HashPassword(string password)
    {
        return hasher.HashPassword(null, password);
    }

    public bool CheckPassword(string? password, string hashedPassword)
    {
        return string.IsNullOrEmpty(password)
            ? false
            : hasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
    }
}
