using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MiniGram.Api;
using MiniGram.Api.Common;
using MiniGram.Api.Storage.Entities;
using MiniGram.Client;
using MiniGram.Client.Utils;
using MiniGram.Contracts.Models;
using Moq;
using Xunit;

namespace MiniGram.Tests.IntegrationTests;

public class CommentPhotoTests : IDisposable
{
    readonly WebApplicationFactory<Program> _app;
    readonly IMiniGramClient _client;
    readonly Mock<ICurrentRequest> _currentRequest = new();
    readonly ClientTestWebAppFactory _factory = new();
    readonly Mock<IMiniGramClient> _mockClient = new(MockBehavior.Strict);

    public CommentPhotoTests()
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
    public async Task Should_add_comment_to_photo()
    {
        var photoId = TestHelper.GetTestPhotoId;
        var comment = new Comment
        {
            PhotoId = photoId,
            Text = "testkommentar"
        };

        _mockClient.Setup(x => x.CommentPhoto(It.IsAny<CommentPhotoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSuccessResponse<Comment>(comment, 201));

        var response = await _client.CommentPhoto(new CommentPhotoRequest
            { PhotoId = photoId, Comment = "testkommentar" });

        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal((int)HttpStatusCode.OK, response.HttpStatus);
    }

    [Fact]
    public async Task Should_NOT_add_comment_to_photo_when_comment_is_too_long()
    {
        var photoId = TestHelper.GetTestPhotoId;
        var fakeComment = string.Concat(Enumerable.Repeat("abc", 1000));
        var comment = new Comment
        {
            PhotoId = photoId,
            Text = fakeComment
        };

        _mockClient.Setup(x => x.CommentPhoto(It.IsAny<CommentPhotoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiErrorResponse<Comment>(comment, 400));

        var response = await _client.CommentPhoto(new CommentPhotoRequest { PhotoId = photoId, Comment = fakeComment });

        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal((int)HttpStatusCode.BadRequest, response.HttpStatus);

        var error = JsonSerializer.Deserialize<ValidationProblemDetails>(response.Error);
        Assert.Equal("Comment is too long.", error.Errors["Comment"].FirstOrDefault());
    }
}
