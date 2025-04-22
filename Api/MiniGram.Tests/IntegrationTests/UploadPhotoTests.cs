using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MiniGram.Api;
using MiniGram.Api.Common;
using MiniGram.Api.Handlers;
using MiniGram.Api.Storage.Entities;
using MiniGram.Client;
using MiniGram.Client.Utils;
using Moq;
using Xunit;

namespace MiniGram.Tests.IntegrationTests;

public class UploadPhotoTests : IDisposable
{
    readonly WebApplicationFactory<Program> _app;
    readonly IMiniGramClient _client;
    readonly Mock<ICurrentRequest> _currentRequest = new();
    readonly ClientTestWebAppFactory _factory = new();
    readonly Mock<IMiniGramClient> _mockClient = new(MockBehavior.Strict);

    public UploadPhotoTests()
    {
        _currentRequest.Setup(x => x.UserId).Returns(TestHelper.GetTestUserId);
        _app = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mockClient.Object);
                services.AddSingleton(_currentRequest.Object);
            });
        });

        _client = _app.CreateServiceClient<MiniGramClient>();
    }

    public void Dispose()
    {
        _factory?.Dispose();
        _app?.Dispose();
    }

    [Fact]
    public async Task Should_upload_image_from_byte_stream()
    {
        var currentSolutionPath = Directory.GetParent("../../../")?.FullName;
        var imagePath = Path.Combine(currentSolutionPath, "Assets", "dogs.jpg");
        var fileContents = await File.ReadAllBytesAsync(imagePath);
        var request = new UploadPhotoRequest
        {
            Contents = fileContents.ToList(),
            ContentType = "image/jpeg",
            Description = "test"
        };

        var inDb = new Photo
        {
            Contents = fileContents,
            UserId = Guid.NewGuid(),
            Description = "test"
        };

        _mockClient.Setup(x => x.UploadPhoto(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSuccessResponse<Photo>(inDb, 201));

        var response = await _client.UploadPhoto(request);

        Assert.Equal((int)HttpStatusCode.Created, response.HttpStatus);
        Assert.Equal(inDb.Contents, response.Item.Contents);
        Assert.Equal(inDb.Description, response.Item.Description);
    }

    [Fact]
    public async Task Should_NOT_upload_image_when_description_is_too_long()
    {
        var currentSolutionPath = Directory.GetParent("../../../")?.FullName;
        var imagePath = Path.Combine(currentSolutionPath, "Assets", "dogs.jpg");
        var fileContents = await File.ReadAllBytesAsync(imagePath);
        var request = new UploadPhotoRequest
        {
            Contents = fileContents.ToList(),
            ContentType = "image/jpeg",
            Description = string.Concat(Enumerable.Repeat("abc", 1000))
        };

        var inDb = new Photo
        {
            Contents = fileContents,
            UserId = Guid.NewGuid(),
            Description = string.Concat(Enumerable.Repeat("abc", 1000))
        };

        _mockClient.Setup(x => x.UploadPhoto(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiErrorResponse<Photo>(inDb, 201));

        var response = await _client.UploadPhoto(request);

        Assert.Equal((int)HttpStatusCode.BadRequest, response.HttpStatus);
        Assert.False(response.IsSuccess);
        var error = JsonSerializer.Deserialize<ValidationProblemDetails>(response.Error);
        Assert.Equal("Description is required or too long.", error.Errors["Description"].FirstOrDefault());
    }

    [Fact]
    public async Task Should_NOT_upload_image_too_large_file()
    {
        var currentSolutionPath = Directory.GetParent("../../../")?.FullName;
        var imagePath = Path.Combine(currentSolutionPath, "Assets", "dogs.jpg");
        var fileContents = await File.ReadAllBytesAsync(imagePath);
        var contents = new byte[fileContents.Length * 100]; // duplicate it 

        for (var i = 0; i < 100; i++)
            Buffer.BlockCopy(fileContents, 0, contents, i * fileContents.Length, fileContents.Length);

        var request = new UploadPhotoRequest
        {
            Contents = contents.ToList(),
            ContentType = "image/jpeg",
            Description = "test"
        };

        var inDb = new Photo
        {
            Contents = contents,
            UserId = Guid.NewGuid(),
            Description = "test"
        };

        _mockClient.Setup(x => x.UploadPhoto(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiErrorResponse<Photo>(inDb, 201));

        var response = await _client.UploadPhoto(request);

        Assert.Equal((int)HttpStatusCode.BadRequest, response.HttpStatus);
        Assert.False(response.IsSuccess);
        var error = JsonSerializer.Deserialize<ValidationProblemDetails>(response.Error);
        Assert.Equal("Contents is too large.", error.Errors["Contents"].FirstOrDefault());
    }
}
