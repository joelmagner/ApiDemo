using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniGram.Api;
using MiniGram.Api.Common;
using MiniGram.Api.Storage.Entities;
using MiniGram.Client;
using MiniGram.Client.Utils;
using Moq;
using Xunit;

namespace MiniGram.Tests.IntegrationTests;

public class GetUserPhotosTests : IDisposable
{
    readonly WebApplicationFactory<Program> _app;
    readonly IMiniGramClient _client;
    readonly Mock<ICurrentRequest> _currentRequest = new();
    readonly ClientTestWebAppFactory _factory = new();
    readonly Mock<IMiniGramClient> _mockClient = new(MockBehavior.Strict);

    public GetUserPhotosTests()
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
    public async Task Should_get_user_photos()
    {
        var userId = TestHelper.GetTestUserId;

        _mockClient.Setup(x => x.GetUserPhotos(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSuccessResponse<Photo[]>(_factory.Db.Context.Photos.ToArray(), 200));

        var response = await _client.GetUserPhotos(userId);
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal((int)HttpStatusCode.OK, response.HttpStatus);
        Assert.Equal(1, response.Item.Length);
    }

    [Fact]
    public async Task Should_get_multiple_user_photos()
    {
        var userId = TestHelper.GetTestUserId;

        await _factory.Db.Context.Photos.AddAsync(new Photo
        {
            Contents = [1, 2, 3, 4],
            Description = "testbild2",
            UserId = TestHelper.GetTestUserId,
            ContentType = "image/jpeg"
        });
        await _factory.Db.Context.SaveChangesAsync();

        _mockClient.Setup(x => x.GetUserPhotos(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSuccessResponse<Photo[]>(_factory.Db.Context.Photos.ToArray(), 200));

        var response = await _client.GetUserPhotos(userId);
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal((int)HttpStatusCode.OK, response.HttpStatus);
        Assert.Equal(2, response.Item.Length);
        Assert.Equal("testbild2", response.Item[1].Description);
    }

    [Fact]
    public async Task Should_return_OK_when_no_photos()
    {
        var userId = TestHelper.GetTestUserId;

        await _factory.Db.Context.Photos
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync();
        await _factory.Db.Context.SaveChangesAsync();

        _mockClient.Setup(x => x.GetUserPhotos(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSuccessResponse<Photo[]>(_factory.Db.Context.Photos.ToArray(), 200));

        var response = await _client.GetUserPhotos(userId);
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal((int)HttpStatusCode.OK, response.HttpStatus);
        Assert.Empty(response.Item);
    }
}
