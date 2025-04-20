using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniGram.Api.Handlers;
using MiniGram.Api.Handlers.Auth;
using MiniGram.Contracts.Models;

namespace MiniGram.Api.Configuration;

public static class RouteExtensions
{
    public static void RegisterRoutes(this IEndpointRouteBuilder route)
    {
        AuthRoutes(route.MapGroup("/auth"));
        UserRoutes(route.MapGroup("/user"));
        UploadRoutes(route.MapGroup("/upload"));
        InteractionRoutes(route.MapGroup("/interaction"));
    }

    static void AuthRoutes(this RouteGroupBuilder route)
    {
        route.MapPost("/login",
                async ([FromBody] LoginRequest request, ILoginHandler handler, CancellationToken cancellationToken) =>
                await handler.Handle(request, cancellationToken))
            .Produces<Ok>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        route.MapPost("/logout", [Authorize]
                async ([FromBody] LogoutRequest request, ILogoutHandler handler, CancellationToken cancellationToken) =>
                await handler.Handle(request, cancellationToken))
            .WithDescription("Not yet implemented")
            .Produces<Ok>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static void UserRoutes(this RouteGroupBuilder route)
    {
        route.MapPost("/create",
                async ([FromBody] CreateUserRequest request, ICreateUserHandler handler,
                        CancellationToken cancellationToken) =>
                    await handler.Handle(request, cancellationToken))
            .Produces<Ok>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);

        route.MapPost("/get", [Authorize] async ([FromBody] GetUserByUsernameRequest request,
                    IGetUserByUsernameHandler handler,
                    CancellationToken cancellationToken) =>
                await handler.Handle(request, cancellationToken))
            .Produces<Ok>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static void UploadRoutes(this RouteGroupBuilder route)
    {
        route.MapPost("/photo",
                async ([FromBody] UploadPhotoRequest request, IUploadPhotoHandler handler,
                        CancellationToken cancellationToken) =>
                    await handler.Handle(request, cancellationToken))
            .Produces<Ok>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }

    static void InteractionRoutes(this RouteGroupBuilder route)
    {
        route.MapPost("/comment",
                async ([FromBody] CommentPhotoRequest request, ICommentPhotoHandler handler,
                        CancellationToken cancellationToken) =>
                    await handler.Handle(request, cancellationToken))
            .Produces<Ok>()
            .Produces<BadRequest>(StatusCodes.Status400BadRequest);
    }
}