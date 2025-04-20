using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MiniGram.Api.Common;
using MiniGram.Api.Handlers;
using MiniGram.Api.Storage.Entities;
using MiniGram.Client;
using MiniGram.Client.Utils;
using Moq;
using Xunit;

namespace MiniGram.Tests.IntegrationTests;

public class CommentPhotoTests
{
    readonly WebApplicationFactory<Program> _app;
    readonly ClientTestWebAppFactory _factory = new();
    readonly Mock<IMiniGramClient> _mockClient = new();
    readonly IMiniGramClient _client;
    readonly Mock<ICurrentRequest> _currentRequest = new();

    public CommentPhotoTests()
    {
        _currentRequest.Setup(x => x.UserId).Returns(TestHelper.GetTestUserId);
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mockClient.Object);
                services.AddSingleton(_currentRequest.Object);
            });
        }).CreateServiceClient<MiniGramClient>();
    }


    [Fact]
    public async Task Should_add_comment_to_photo()
    {
        var photoId = TestHelper.GetTestPhotoId;
        var comment = new Comment
        {
            PhotoId = photoId,
            Text = "testkommentar",
        };

        _mockClient.Setup(x => x.CommentPhoto(It.IsAny<CommentPhotoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSuccessResponse<Comment>(comment, 201));

        var response =
            await _client.CommentPhoto(new CommentPhotoRequest { PhotoId = photoId, Comment = "testkommentar" },
                default);

        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(comment.Text, response.Item.Text);
    }
    
    [Fact]
    public async Task Should_NOT_add_comment_to_photo_when_comment_is_too_long()
    {
        var photoId = TestHelper.GetTestPhotoId;
        var comment = new Comment
        {
            PhotoId = photoId,
            Text = string.Concat(Enumerable.Repeat("abc", 1000))
        };

        _mockClient.Setup(x => x.CommentPhoto(It.IsAny<CommentPhotoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSuccessResponse<Comment>(comment, 201));

        var response =
            await _client.CommentPhoto(new CommentPhotoRequest { PhotoId = photoId, Comment = string.Concat(Enumerable.Repeat("abc", 1000)) },
                default);

        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal((int)HttpStatusCode.BadRequest, response.HttpStatus);
        
        var error = System.Text.Json.JsonSerializer.Deserialize<ValidationProblemDetails>(response.Error);
        Assert.Equal("Comment is too long.", error.Errors["Comment"].FirstOrDefault());
    }
}