using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniGram.Api.Common;
using MiniGram.Api.Handlers;
using MiniGram.Api.Handlers.Auth;
using MiniGram.Api.Storage;
using MiniGram.Api.Utils;

namespace MiniGram.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<MiniGramContext>(tags: [nameof(MiniGramContext)]);
        services.AddMemoryCache();
        services.AddHttpContextAccessor(); // for setting the AccessToken in the HttpsContext
        services.AddResponseCompression();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:4200") // Assuming Angular runs on this port.
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // Allows credentials (cookies, authorization headers, etc.)
            });
        });

        services.AddDbContext<MiniGramContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("MiniGram"));
        });
        services.AddAuthentication();

        services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentRequest, CurrentRequest>();

        services.AddScoped<IValidator, Validator>();

        // handlers
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<IMeHandler, MeHandler>();
        services.AddScoped<IGetUserByUsernameHandler, GetUserByUsernameHandler>();
        services.AddScoped<IGetUserPhotosHandler, GetUserPhotosHandlerHandler>();
        services.AddScoped<ILoginHandler, LoginHandler>();
        services.AddScoped<ILogoutHandler, LogoutHandler>();
        services.AddScoped<ICommentPhotoHandler, CommentPhotoHandler>();
        services.AddScoped<IUploadPhotoHandler, UploadPhotoHandler>();

        return services;
    }

    public static IServiceCollection AddJwtBearer(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection("Jwt"));

        services.AddSingleton<JwtConfig>(sp =>
            sp.GetRequiredService<IOptions<JwtConfig>>().Value);


        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"]
                };


                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers.Authorization
                            .FirstOrDefault()?.Split(" ").Last();
                        if (!string.IsNullOrEmpty(token)) context.Token = token;

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"JWT Auth failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"JWT Auth failed: {context.Result}");
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
