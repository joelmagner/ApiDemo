using Microsoft.AspNetCore.Identity;
using MiniGram.Api.Common;
using MiniGram.Api.Storage.Entities;
using Xunit;

namespace MiniGram.Tests.UnitTests;

public class AuthServiceTests
{
    static readonly IPasswordHasher<object> PasswordHasher = new PasswordHasher<object>();
    readonly AuthService _authService = new(PasswordHasher);
    
    [Fact]
    public void Should_serialize_and_deserialize_credentials()
    {
        // Arrange
        var credentials = new Credentials
        {
            UserId = Guid.NewGuid(),
            Password = "password123",
            Created = DateTime.UtcNow,
        };

        // Act
        var hashedPassword = _authService.HashPassword(credentials.Password);
        
        // Assert
        Assert.NotNull(hashedPassword);
        Assert.False(_authService.CheckPassword(credentials.Password+"NotActualPassword", hashedPassword));
        Assert.True(_authService.CheckPassword(credentials.Password, hashedPassword));
        Assert.False(_authService.CheckPassword("", hashedPassword));
        Assert.False(_authService.CheckPassword(null, hashedPassword));
    }
    
}