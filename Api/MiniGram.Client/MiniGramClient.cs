using System.Net.Http.Json;
using System.Text.Json;
using MiniGram.Api.Handlers;
using MiniGram.Api.Storage.Entities;
using MiniGram.Client.Utils;
using MiniGram.Contracts.Models;

namespace MiniGram.Client;

public interface IMiniGramClient
{
    Task<Response<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Response<string>> Logout(LogoutRequest request, CancellationToken cancellationToken = default);
    Task<Response<User>> CreateUser(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Response<User>> GetUserByUsername(GetUserByUsernameRequest request, CancellationToken cancellationToken = default);
    Task<Response<Photo>> UploadPhoto(UploadPhotoRequest request, CancellationToken cancellationToken = default);
    Task<Response<Comment>> CommentPhoto(CommentPhotoRequest request, CancellationToken cancellationToken = default);
}


public class MiniGramClient(HttpClient client) : IMiniGramClient
{
    readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

    public async Task<Response<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync("/auth/login", request, _options, cancellationToken);
        return await response.ToClientItemResponse<LoginResponse>(cancellationToken);
    }
    
    public async Task<Response<string>> Logout(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync("/auth/logout", request, _options, cancellationToken);
        return await response.ToClientItemResponse<string>(cancellationToken);
    }

    public async Task<Response<User>> CreateUser(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync("/user/create", request, _options, cancellationToken);
        return await response.ToClientItemResponse<User>(cancellationToken);
    }

    public async Task<Response<User>> GetUserByUsername(GetUserByUsernameRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync("/user/get", request, _options, cancellationToken);
        return await response.ToClientItemResponse<User>(cancellationToken);
    }
    
    public async Task<Response<Photo>> UploadPhoto(UploadPhotoRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync("/upload/photo", request, _options, cancellationToken);
        return await response.ToClientItemResponse<Photo>(cancellationToken);
    }
    
    public async Task<Response<Comment>> CommentPhoto(CommentPhotoRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync("/interaction/comment", request, _options, cancellationToken);
        return await response.ToClientItemResponse<Comment>(cancellationToken);
    }
}